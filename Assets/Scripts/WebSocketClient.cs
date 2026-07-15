using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    ClientWebSocket socket;

    async void Start()
    {
        socket = new ClientWebSocket();

        await socket.ConnectAsync(new Uri("ws://localhost:5151/ws"), CancellationToken.None);

        Debug.Log("Connected");

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