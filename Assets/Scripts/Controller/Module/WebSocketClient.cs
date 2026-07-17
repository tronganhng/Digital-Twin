using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;

public class WebSocketClient : SimulationBaseService
{
    ClientWebSocket socket;
    readonly Dictionary<string, TaskCompletionSource<string>> pendingResponses = new Dictionary<string, TaskCompletionSource<string>>();

    public async Task InitWebSocket(SimulationManager manager)
    {
        base.Init(manager);

        socket = new ClientWebSocket();

        await socket.ConnectAsync(new Uri("ws://localhost:5055/ws"), CancellationToken.None);

        ExtraLog.LogWithColor("Websocket Connected!", Color.turquoise);

        ReceiveLoop();
    }

    async void ReceiveLoop()
    {
        byte[] buffer = new byte[4096];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            string prettyJson = JToken.Parse(json).ToString(Formatting.Indented);
            Debug.Log(prettyJson);

            try
            {
                var message = JsonConvert.DeserializeObject<SocketMessage<JObject>>(json);
                if (message == null)
                    continue;

                if (!string.IsNullOrWhiteSpace(message.RequestId) &&
                    pendingResponses.TryGetValue(message.RequestId, out var tcs))
                {
                    tcs.TrySetResult(json);
                    pendingResponses.Remove(message.RequestId);
                    continue;
                }

                RouteIncomingMessage(message);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to parse socket message: {ex.Message}\n{json}");
            }
        }
    }

    private void RouteIncomingMessage(SocketMessage<JObject> message)
    {
        switch (message.Type)
        {
            case SocketMessageType.TaskAssigned:
                HandleTaskAssigned(message.Payload?.ToObject<DeliveryTask>());
                break;
            default:
                Debug.LogWarning($"Unhandled socket message type: {message.Type}");
                break;
        }
    }

    private void HandleTaskAssigned(DeliveryTask task)
    {
        if (task == null)
            return;

        manager.TaskManager.UpdateTaskInfo(task);
        var robot = manager.RobotManager.GetRobotBy(task.AssignedRobotId);
        robot.TaskModule.DoTask(task);
        ExtraLog.LogWithColor($"Task assigned: {task.TaskId}", Color.cyan);
    }

    public async Task SendMessageAsync<T>(SocketMessageType type, T payload)
    {
        if (socket == null || socket.State != WebSocketState.Open)
            return;

        var message = new SocketMessage<T>
        {
            Type = type,
            RequestId = Guid.NewGuid().ToString(),
            Payload = payload
        };

        string json = JsonConvert.SerializeObject(message);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(SocketMessageType type, TRequest payload, CancellationToken cancellationToken = default, int timeoutMilliseconds = 10000)
    {
        // Kiểm tra trạng thái socket an toàn
        if (socket == null || socket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket chưa được kết nối hoặc đã đóng.");
        }

        var requestId = Guid.NewGuid().ToString();
        var request = new SocketMessage<TRequest>
        {
            Type = type,
            RequestId = requestId,
            Payload = payload
        };

        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        pendingResponses[requestId] = tcs;

        try
        {
            string json = JsonConvert.SerializeObject(request);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            // 2. Đồng bộ hóa việc gửi tin nhắn (Bảo vệ luồng nếu cần)
            // Nếu hệ thống gọi hàm này rất nhiều, bạn nên bọc socket.SendAsync bằng một SemaphoreSlim để tránh lỗi đồng thời.
            await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);

            // 3. Cơ chế tạo Timeout kết hợp với CancellationToken
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                cts.CancelAfter(timeoutMilliseconds);

                // Đăng ký hành động nếu bị hủy hoặc hết hạn thì hủy bỏ Task tương ứng
                using (cts.Token.Register(() => tcs.TrySetCanceled()))
                {
                    // Chờ phản hồi từ Server
                    string responseJson = await tcs.Task;

                    // 4. Khắc phục việc Deserialize an toàn
                    var responseMessage = JsonConvert.DeserializeObject<SocketMessage<TResponse>>(responseJson);

                    if (responseMessage == null || responseMessage.Payload == null)
                    {
                        throw new JsonException("Phản hồi từ Server không đúng cấu trúc kì vọng.");
                    }

                    return responseMessage.Payload;
                }
            }
        }
        catch (TaskCanceledException)
        {
            throw new TimeoutException($"Yêu cầu loại {type} (ID: {requestId}) đã quá hạn {timeoutMilliseconds}ms hoặc bị hủy.");
        }
        finally
        {
            pendingResponses.Remove(requestId, out _);
        }
    }

    [Button]
    private async void TestSend()
    {
        var robot = new RobotStateDto { RobotId = "latgoto" };
        var response = await SendRequestAsync<RobotStateDto, RobotStateDto>(SocketMessageType.RegisterRobot, robot);
        Debug.Log("Get RobotID from Server: " + response.RobotId);
    }
}