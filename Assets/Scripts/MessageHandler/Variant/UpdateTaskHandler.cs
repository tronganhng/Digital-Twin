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
        var robot = SimulationManager.Instance.RobotManager.GetRobotBy(task.AssignedRobotId);
        robot.StatModule.SetTaskId(task.TaskId);
    }
}