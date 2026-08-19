using UnityEngine;
using Newtonsoft.Json.Linq;

public class StopRobotHandler : BaseMessageHandler
{
    public override void Handle(SocketMessage<JToken> message)
    {
        var robot = SimulationManager.Instance.RobotManager.GetRobotBy(message.RobotId);
        if (robot != null)
        {
            robot.StateMachine.ChangeState(new IdleState(robot.StateMachine));
        }
    }
}