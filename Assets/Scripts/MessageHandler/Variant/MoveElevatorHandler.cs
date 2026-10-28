using UnityEngine;
using Newtonsoft.Json.Linq;

public class MoveElevatorHandler : BaseMessageHandler
{
    public override void Handle(SocketMessage<JToken> message)
    {
        var movePayload = message.Payload?.ToObject<MoveElevatorRequest>();
        if (movePayload == null) return;

        var elevator = SimulationManager.Instance.MapManager.GetElevatorBy(movePayload.ElevatorId);
        if (elevator != null)
        {
            elevator.GoToFloor(movePayload.TargetFloor);
        }
    }
}