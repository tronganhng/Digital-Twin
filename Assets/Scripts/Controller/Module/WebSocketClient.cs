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
using System.Collections.Concurrent;
using Newtonsoft.Json.Converters;

public class WebSocketClient : SimulationBaseService
{
    private ClientWebSocket socket;
    private CancellationTokenSource receiveCts;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> pendingResponses = new();

    public async Task InitWebSocket(SimulationManager manager)
    {
        base.Init(manager);

        socket = new ClientWebSocket();
        receiveCts = new CancellationTokenSource();

        await socket.ConnectAsync(new Uri("ws://localhost:5055/ws"), CancellationToken.None);

        await SendMessageAsync(SocketMessageType.RegisterClient, ClientType.Unity);

        await SetSystemMode(SystemMode.Simulation);

        _ = ReceiveLoop(receiveCts.Token);
    }

    private async Task ReceiveLoop(CancellationToken token)
    {
        byte[] buffer = new byte[4096];

        while (!token.IsCancellationRequested && socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            string prettyJson = JToken.Parse(json).ToString(Formatting.Indented);
            ExtraLog.Log(prettyJson);

            try
            {
                var message = JsonConvert.DeserializeObject<SocketMessage<JToken>>(json);
                if (message == null)
                    continue;

                if (!string.IsNullOrWhiteSpace(message.RequestId) &&
                    pendingResponses.TryGetValue(message.RequestId, out var tcs))
                {
                    tcs.TrySetResult(json);
                    pendingResponses.TryRemove(message.RequestId, out _);
                    continue;
                }

                manager.Router.Route(message);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to parse socket message: {ex.Message}\n{json}");
            }
        }
    }

    public async Task SendMessageAsync<T>(SocketMessageType type, T payload, string robotId = null)
    {
        if (socket == null || socket.State != WebSocketState.Open)
            return;

        var message = new SocketMessage<T>
        {
            Type = type,
            RequestId = Guid.NewGuid().ToString(),
            RobotId = robotId,
            Payload = payload
        };

        string json = JsonConvert.SerializeObject(message, new JsonSerializerSettings
        {
            Converters = { new StringEnumConverter() }
        });
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(SocketMessageType type, TRequest payload, CancellationToken cancellationToken = default, int timeoutMilliseconds = 5000)
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
            string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                Converters = { new StringEnumConverter() }
            });
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
            pendingResponses.TryRemove(requestId, out _);
        }
    }

    public async Task Disconnect()
    {
        receiveCts?.Cancel();

        if (socket != null && socket.State == WebSocketState.Open)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Unity Exit", CancellationToken.None);
        }

        socket?.Dispose();
        receiveCts?.Dispose();
        ExtraLog.LogWithColor("Disconected", Color.softRed);
    }

    [Button]
    private async Task SetSystemMode(SystemMode mode)
    {
        if (mode == SystemMode.Operation)
        {
            manager.TaskManager.ClearAllTask();
            manager.RobotManager.ClearAllRobots();
        }
        await SendMessageAsync(SocketMessageType.SetSystemMode, mode);
    }
}