using UnityEngine;

public class Robot : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private RobotStatus status = RobotStatus.Idle;
    [SerializeField] private float battery = 100;

    [field: SerializeField]
    public RobotMoveModule MoveModule { get; private set; }

    public int RobotId { get; private set; }

    public RobotTask CurrentTask { get; private set; }

    public RobotStatus Status => status;
    public float Battery => battery;

    private bool _pickupCompleted;

    public void Init(int robotId)
    {
        RobotId = robotId;
        MoveModule.Init(this);
    }

    public void ApplyState(RobotStateDto state)
    {
        transform.SetPositionAndRotation(state.position, state.rotation);
    }

    public bool AssignTask(RobotTask task)
    {
        if (status != RobotStatus.Idle)
            return false;

        CurrentTask = task;
        _pickupCompleted = false;

        status = RobotStatus.MovingToPickup;

        Vector3 target = FleetManager.Instance.MapManager.GetPoint(task.PickupPoint).Position;

        MoveModule.MoveTo(target);

        return true;
    }

    public void OnDestinationReached()
    {
        switch (status)
        {
            case RobotStatus.MovingToPickup:

                status = RobotStatus.Loading;

                // Demo: load xong ngay
                FinishLoading();

                break;

            case RobotStatus.MovingToDestination:

                status = RobotStatus.Unloading;

                // Demo: unload xong ngay
                FinishUnloading();

                break;
        }
    }

    private void FinishLoading()
    {
        _pickupCompleted = true;

        status = RobotStatus.MovingToDestination;

        Vector3 target = FleetManager.Instance.MapManager.GetPoint(CurrentTask.DestinationPoint).Position;

        MoveModule.MoveTo(target);
    }

    private void FinishUnloading()
    {
        status = RobotStatus.Idle;

        CurrentTask = null;
    }
}