using Sirenix.OdinInspector;
using UnityEngine;

public class FleetManager : MonoSingleton<FleetManager>
{
    [field: SerializeField] public RobotManager RobotManager { get; private set; }
    [field: SerializeField] public TaskManager TaskManager { get; private set; }
    [field: SerializeField] public Scheduler Scheduler { get; private set; }
    [field: SerializeField] public MapManager MapManager { get; private set; }
    [field: SerializeField] public TrafficManager TrafficManager { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        RobotManager.Init(this);
        TaskManager.Init(this);
        Scheduler.Init(this);
        MapManager.Init(this);
        TrafficManager.Init(this);
    }

    [Title("Test")]
    [SerializeField] string pickupPoint;
    [SerializeField] string destinationPoint;

    [Button]
    private void AssignTask()
    {
        RobotTask task = TaskManager.CreateTask(pickupPoint, destinationPoint);
        Robot bestRobot = Scheduler.FindBestRobot(task);
        if (bestRobot != null && bestRobot.AssignTask(task))
        {
            TaskManager.AssignTask(task.Id, bestRobot.StatModule.RobotId);
        }
        else
        {
            Debug.LogWarning("No suitable robot found for the task.");
        }
    }
}