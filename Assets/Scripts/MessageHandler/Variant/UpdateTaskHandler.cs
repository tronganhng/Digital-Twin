using Newtonsoft.Json.Linq;
using UnityEngine;

public class UpdateTaskHandler : BaseMessageHandler
{
    public override void Handle(SocketMessage<JToken> message)
    {
        var task = message.Payload?.ToObject<DeliveryTask>();
        if (task == null)
            return;

        SimulationManager.Instance.TaskManager.UpdateTaskInfo(task);
        if (task.AssignedRobotId == null)
            return;
        var robot = SimulationManager.Instance.RobotManager.GetRobotBy(task.AssignedRobotId);
        robot.StatModule.SetTaskId(task.TaskId);
        if (task.Status == TaskStatus.Cancelled) robot.StateMachine.ChangeState(new IdleState(robot.StateMachine));
    }
}