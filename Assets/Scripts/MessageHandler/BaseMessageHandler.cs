using UnityEngine;
using Newtonsoft.Json.Linq;

public abstract class BaseMessageHandler : MonoBehaviour
{
    public abstract void Handle(SocketMessage<JToken> message);
}