public enum RobotStatus
{
    Idle,
    Moving,
    Charging,
    Error,
    Offline
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

public enum NodeType
{
    Room,
    SharedResource,
}

public enum SocketMessageType
{
    ServerResponse,
    RegisterRobot,
    RobotState,
    CreateTask,
    TaskAssigned,
    UpdateTask,
    CancelTask,
    ResourceAccess,
    ResourceRelease,
    AcquireDockPoint,
    ReleaseDockPoint,
    CheckNodeFull,
    MoveRobot,
    RobotArrived,
}