public enum RobotStatus
{
    Idle,
    MovingToPickup,
    Loading,
    MovingToDestination,
    Unloading,
    Charging,
    Error
}

public enum TaskStatus
{
    Pending,
    Assigned,
    Running,
    Completed,
    Failed,
    Cancelled
}