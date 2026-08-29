using UnityEngine;
using Newtonsoft.Json.Linq;

public class SyncRobotStateHandler : BaseMessageHandler
{
    public override void Handle(SocketMessage<JToken> message)
    {
        var robotDto = message.Payload?.ToObject<RobotStateDto>();
        if (robotDto == null) return;

        var robot = SimulationManager.Instance.RobotManager.GetRobotBy(message.RobotId);
        robot.StatModule.SyncData(robotDto);
    }
}