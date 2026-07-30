using System.Threading.Tasks;
using Sigtrap.Relays;
using Sirenix.OdinInspector;
using UnityEngine;

public class RobotTaskModule : RobotBaseModule
{
    [field: SerializeField, ReadOnly] public DeliveryTask CurrentTask { get; private set; }

    [ReadOnly] public MapNode PickupNode;
    [ReadOnly] public MapNode DesNode;

    public Relay OnTaskStart = new();

    public void DoTask(DeliveryTask task)
    {
        CurrentTask = task;
        robot.StatModule.SetTaskId(task.TaskId);

        var mapManager = SimulationManager.Instance.MapManager;
        if (mapManager.TryGetNode(task.PickupLocation, out PickupNode) && mapManager.TryGetNode(task.Destination, out DesNode))
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
        if (CurrentTask == null) return;

        CurrentTask.Status = status;
        CurrentTask.OnDataChanged.Dispatch();
        _ = SendTaskAsync();

        if (status == TaskStatus.Completed || status == TaskStatus.Cancelled)
        {
            robot.StatModule.SetTaskId(null);
            CurrentTask = null;
            PickupNode = null;
            DesNode = null;
            robot.StateMachine.ChangeState(new IdleState(robot.StateMachine));
        }
    }

    private async Task SendTaskAsync()
    {
        await SimulationManager.Instance.WebSocket.SendMessageAsync(SocketMessageType.UpdateTask, CurrentTask);
    }
}