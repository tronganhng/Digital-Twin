using Sigtrap.Relays;
using Sirenix.OdinInspector;
using UnityEngine;

public class RobotTaskModule : RobotBaseModule
{
    [field: SerializeField, ReadOnly] public DeliveryTask CurrentTask { get; private set; }

    public MapPoint PickupPoint;
    public MapPoint DesPoint;

    public Relay OnTaskStart = new();

    public void DoTask(DeliveryTask task)
    {
        CurrentTask = task;
        robot.StatModule.SetTaskId(task.TaskId);

        var mapManager = SimulationManager.Instance.MapManager;
        if (mapManager.TryGetPoint(task.PickupLocation, out PickupPoint) && mapManager.TryGetPoint(task.Destination, out DesPoint))
        {
            robot.MoveModule.MoveTo(PickupPoint.Position, OnReachPickupPos);
        }
        else
        {

        }
    }

    private void OnReachPickupPos()
    {
        robot.MoveModule.MoveTo(DesPoint.Position);
    }
}