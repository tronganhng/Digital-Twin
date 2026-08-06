using UnityEngine;
using Newtonsoft.Json.Linq;

public class ChangeRobotStatusHandler : BaseMessageHandler
{
    public override void Handle(SocketMessage<JToken> message)
    {
        if (message.Payload == null)
            return;

        var status = message.Payload.ToObject<RobotStatus>();
        var robot = SimulationManager.Instance.RobotManager.GetRobotBy(message.RobotId);
        if (robot != null)
        {
            robot.StatModule.SetStatus(status);
            if (status == RobotStatus.Idle)
            {
                robot.StateMachine.ChangeState(new IdleState(robot.StateMachine));
            }
        }
    }
}