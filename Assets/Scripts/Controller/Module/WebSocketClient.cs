using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketClient : SimulationBaseService
{
    ClientWebSocket socket;

    public async Task InitWebSocket(SimulationManager manager)
    {
        base.Init(manager);

        socket = new ClientWebSocket();

        await socket.ConnectAsync(new Uri("ws://localhost:5055/ws"), CancellationToken.None);

        Debug.Log("Websocket Connected!");

        ReceiveLoop();
    }

    async void ReceiveLoop()
    {
        byte[] buffer = new byte[1024];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Debug.Log(json);
        }
    }
}