using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

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

            var baseMessage = JsonConvert.DeserializeObject<SocketMessage<object>>(json);
            if (baseMessage?.RequestId != null && pendingResponses.TryGetValue(baseMessage.RequestId, out var tcs))
            {
                tcs.TrySetResult(json);
                pendingResponses.Remove(baseMessage.RequestId);
                continue;
            }

            // Debug.Log(json);
        }
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

    public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(SocketMessageType type, TRequest payload)
    {
        if (socket == null || socket.State != WebSocketState.Open)
            return default!;

        var requestId = Guid.NewGuid().ToString();
        var request = new SocketMessage<TRequest>
        {
            Type = type,
            RequestId = requestId,
            Payload = payload
        };

        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        pendingResponses[requestId] = tcs;

        string json = JsonConvert.SerializeObject(request);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

        string responseJson = await tcs.Task;
        var responseMessage = JsonConvert.DeserializeObject<SocketMessage<TResponse>>(responseJson);
        return responseMessage.Payload;
    }

    [Button]
    private async void TestSend()
    {
        var robot = new RobotStateDto { RobotId = "latgoto" };
        var response = await SendRequestAsync<RobotStateDto, RobotStateDto>(SocketMessageType.RegisterRobot, robot);
        Debug.Log("Get RobotID from Server: " + response.RobotId);
    }
}