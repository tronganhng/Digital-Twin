using System.Threading.Tasks;
using Sigtrap.Relays;
using Sirenix.OdinInspector;
using UnityEngine;

public class RobotTaskModule : RobotBaseModule
{
    [field: SerializeField, ReadOnly] public DeliveryTask CurrentTask { get; private set; }

    [ReadOnly] public MapPoint PickupPoint;
    [ReadOnly] public MapPoint DesPoint;

    public Relay OnTaskStart = new();

    public void DoTask(DeliveryTask task)
    {
        CurrentTask = task;
        robot.StatModule.SetTaskId(task.TaskId);

        var mapManager = SimulationManager.Instance.MapManager;
        if (mapManager.TryGetPoint(task.PickupLocation, out PickupPoint) && mapManager.TryGetPoint(task.Destination, out DesPoint))
        {
            OnTaskStart.Dispatch();
        }
        else
        {
            ExtraLog.LogWithColor("Invalid Point! Maybe u forgot sync Map on client & server", Color.red);
        }
    }

    public void SetTaskStatus(TaskStatus status)
    {
        CurrentTask.Status = status;
        _ = SendTaskAsync();
    }

    private async Task SendTaskAsync()
    {
        await SimulationManager.Instance.WebSocket.SendMessageAsync(SocketMessageType.UpdateTask, CurrentTask);
    }
}