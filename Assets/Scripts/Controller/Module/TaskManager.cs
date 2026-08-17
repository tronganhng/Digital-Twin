using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

public class TaskManager : SimulationBaseService
{
    [SerializeField, ReadOnly] private List<DeliveryTask> tasks;

    public void UpdateTaskInfo(DeliveryTask newTaskInfo)
    {
        int index = tasks.FindIndex(t => t.TaskId == newTaskInfo.TaskId);

        if (index == -1)
        {
            manager.GUI.Dashboard.AddTask(newTaskInfo);
            tasks.Add(newTaskInfo);
        }
        else
        {
            manager.GUI.Dashboard.ReplaceTask(newTaskInfo);
            tasks[index] = newTaskInfo;
        }
        newTaskInfo.OnDataChanged.Dispatch();
    }

    public async void CancelTask(DeliveryTask task)
    {
        if (task == null || !tasks.Contains(task)) return;

        await manager.WebSocket.SendMessageAsync(SocketMessageType.CancelTask, task);

        // if (res == null) return;

        // var robot = manager.RobotManager.GetRobotByTask(res.TaskId);
        // if (robot != null)
        //     robot.StateMachine.ChangeState(new IdleState(robot.StateMachine));
    }

    public async void CreateTask(string pickPoint, string destination, int priority)
    {
        var task = new DeliveryTask
        {
            PickupLocation = pickPoint,
            Destination = destination,
            Priority = priority,
        };

        await manager.WebSocket.SendMessageAsync(SocketMessageType.CreateTask, task);
    }
}