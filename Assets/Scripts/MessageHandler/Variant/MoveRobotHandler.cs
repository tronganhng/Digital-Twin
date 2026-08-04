using UnityEngine;
using Newtonsoft.Json.Linq;

public class MoveRobotHandler : BaseMessageHandler
{
    public override void Handle(SocketMessage<JToken> message)
    {
        var pos = message.Payload?.ToObject<float[]>();
        if (pos == null)
            return;

        var robot = SimulationManager.Instance.RobotManager.GetRobotBy(message.RobotId);
        if (robot != null)
        {
            robot.StateMachine.ChangeState(new MoveState(robot.StateMachine, new Vector3(pos[0], 0, pos[1])));
        }
    }
}