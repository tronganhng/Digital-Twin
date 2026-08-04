using System.Threading.Tasks;
using Sigtrap.Relays;
using Sirenix.OdinInspector;
using UnityEngine;

public class RobotTaskModule : RobotBaseModule
{
    [SerializeField, ReadOnly] private DeliveryTask currentTask;
    [SerializeField, ReadOnly] private MapNode PickupNode;
    [SerializeField, ReadOnly] private MapNode DesNode;

    public void SetTask(DeliveryTask task)
    {
        currentTask = task;
        robot.StatModule.SetTaskId(task.TaskId);

        var mapManager = SimulationManager.Instance.MapManager;
        if (mapManager.TryGetNode(task.PickupLocation, out PickupNode) && mapManager.TryGetNode(task.Destination, out DesNode))
        {

        }
        else
        {
            ExtraLog.LogWithColor("Invalid Point! Maybe u forgot sync Map on client & server", Color.red);
        }
    }
}