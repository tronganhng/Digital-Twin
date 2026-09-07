using UnityEngine;
using Newtonsoft.Json.Linq;

public class ChargeRobotHandler : BaseMessageHandler
{
    public override void Handle(SocketMessage<JToken> message)
    {
        var robot = SimulationManager.Instance.RobotManager.GetRobotBy(message.RobotId);
        if (robot == null) return;

        robot.GoCharge();
    }
}