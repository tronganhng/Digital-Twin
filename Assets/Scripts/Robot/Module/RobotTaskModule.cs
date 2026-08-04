using System.Threading.Tasks;
using Sigtrap.Relays;
using Sirenix.OdinInspector;
using UnityEngine;

public class RobotTaskModule : RobotBaseModule
{
    [field: SerializeField, ReadOnly] public DeliveryTask CurrentTask { get; private set; }

    [ReadOnly] public MapNode PickupNode;
    [ReadOnly] public MapNode DesNode;

    public void DoTask(DeliveryTask task)
    {
        CurrentTask = task;
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