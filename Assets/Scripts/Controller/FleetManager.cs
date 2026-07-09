using UnityEngine;

public class FleetManager : MonoSingleton<FleetManager>
{
    [field: SerializeField] public RobotManager RobotManager { get; private set; }
    [field: SerializeField] public TaskManager TaskManager { get; private set; }
    [field: SerializeField] public Scheduler Scheduler { get; private set; }
    [field: SerializeField] public MapManager MapManager { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        RobotManager.Init(this);
        TaskManager.Init(this);
        Scheduler.Init(this);
        MapManager.Init(this);
    }
}