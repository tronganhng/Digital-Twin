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
    UpdateTask,
    CancelTask,
    ResourceAccess,
    ResourceRelease,
    CheckNodeFull,
    MoveRobot,
    RobotArrived,
    ChangeRobotStatus,
}