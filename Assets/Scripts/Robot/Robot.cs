using UnityEngine;

public class Robot : MonoBehaviour
{
    [field: SerializeField] public RobotMoveModule MoveModule { get; private set; }
    [field: SerializeField] public RobotStatModule StatModule { get; private set; }

    public RobotTask CurrentTask { get; private set; }

    public void Init(int robotId)
    {
        MoveModule.Init(this);
        StatModule.Init(this, robotId);
    }

    public void ApplyState(RobotStateDto state)
    {
        transform.SetPositionAndRotation(state.position, state.rotation);
    }

    public bool AssignTask(RobotTask task)
    {
        if (StatModule.Status != RobotStatus.Idle)
            return false;

        CurrentTask = task;

        StatModule.Status = RobotStatus.MovingToPickup;

        Vector3 target = FleetManager.Instance.MapManager.GetPoint(task.PickupPoint).Position;

        MoveModule.MoveTo(target, OnDestinationReached);

        return true;
    }

    private void OnDestinationReached()
    {
        switch (StatModule.Status)
        {
            case RobotStatus.MovingToPickup:

                StatModule.Status = RobotStatus.Loading;

                // Demo: load xong ngay
                FinishLoading();

                break;

            case RobotStatus.MovingToDestination:

                StatModule.Status = RobotStatus.Unloading;

                // Demo: unload xong ngay
                FinishUnloading();

                break;
        }
    }

    private void FinishLoading()
    {
        StatModule.Status = RobotStatus.MovingToDestination;

        Vector3 target = FleetManager.Instance.MapManager.GetPoint(CurrentTask.DestinationPoint).Position;

        MoveModule.MoveTo(target);
    }

    private void FinishUnloading()
    {
        StatModule.Status = RobotStatus.Idle;

        CurrentTask = null;
    }
}