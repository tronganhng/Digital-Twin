using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.Rendering;

public class MessageRouter : SimulationBaseService
{
    [SerializeField] private SerializedDictionary<SocketMessageType, BaseMessageHandler> _handlers;

    public void Route(SocketMessage<JToken> message)
    {
        if (!_handlers.TryGetValue(message.Type, out var handler))
            return;

        handler.Handle(message);
    }
}