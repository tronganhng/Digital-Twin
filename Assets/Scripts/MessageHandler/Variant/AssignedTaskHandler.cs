using Newtonsoft.Json.Linq;
using UnityEngine;

public class AssignedTaskHandler : BaseMessageHandler
{
    public override void Handle(SocketMessage<JObject> message)
    {
        var task = message.Payload?.ToObject<DeliveryTask>();
        if (task == null)
            return;

        SimulationManager.Instance.TaskManager.UpdateTaskInfo(task);
        var robot = SimulationManager.Instance.RobotManager.GetRobotBy(task.AssignedRobotId);
        robot.TaskModule.DoTask(task);
        ExtraLog.LogWithColor($"Task assigned to: {task.AssignedRobotId}", Color.cyan);
    }
}