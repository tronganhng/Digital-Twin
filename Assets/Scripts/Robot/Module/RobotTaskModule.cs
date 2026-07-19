using Sirenix.OdinInspector;
using UnityEngine;

public class RobotTaskModule : RobotBaseModule
{
    [field: SerializeField, ReadOnly] public DeliveryTask CurrentTask { get; private set; }

    private MapPoint _pickupPoint, _desPoint;

    public void DoTask(DeliveryTask task)
    {
        CurrentTask = task;
        robot.StatModule.SetTaskId(task.TaskId);

        var mapManager = SimulationManager.Instance.MapManager;
        if (mapManager.TryGetPoint(task.PickupLocation, out _pickupPoint) && mapManager.TryGetPoint(task.Destination, out _desPoint))
        {
            robot.MoveModule.MoveTo(_pickupPoint.Position, OnReachPickupPos);
        }
        else
        {

        }
    }

    private void OnReachPickupPos()
    {
        robot.MoveModule.MoveTo(_desPoint.Position);
    }
}