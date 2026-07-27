using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

public class TaskManager : SimulationBaseService
{
    [SerializeField, ReadOnly] private List<DeliveryTask> tasks;

    #region Test
    [SerializeField, TabGroup("Create")] private MapManager map;
    [SerializeField, TabGroup("Create"), ValueDropdown(nameof(GetPointNames))] string pickupPoint;
    [SerializeField, TabGroup("Create"), ValueDropdown(nameof(GetPointNames))] string destinationPoint;
    [SerializeField, TabGroup("Create")] int priority;
    [SerializeField, TabGroup("Cancel"), ValueDropdown(nameof(GetTaskIds))] private string cancelTaskId;

    private IEnumerable<string> GetPointNames()
    {
        return map.GetPointNames();
    }

    [Button(ButtonSizes.Medium), TabGroup("Create")]
    private void PickRandom()
    {
        var names = map.GetPointNames().ToList();

        if (names.Count < 2)
        {
            Debug.LogWarning("Need at least 2 map points.");
            return;
        }

        int pickupIndex = Random.Range(0, names.Count);

        int destinationIndex;
        do
        {
            destinationIndex = Random.Range(0, names.Count);
        }
        while (destinationIndex == pickupIndex);

        pickupPoint = names[pickupIndex];
        destinationPoint = names[destinationIndex];
    }

    [Button(ButtonSizes.Medium), TabGroup("Create"), GUIColor(0.2f, 1f, 0.3f)]
    private async void AssignTask()
    {
        var task = new DeliveryTask
        {
            PickupLocation = pickupPoint,
            Destination = destinationPoint,
            Priority = priority,
        };

        var res = await manager.WebSocket.SendRequestAsync<DeliveryTask, DeliveryTask>(SocketMessageType.CreateTask, task);
        if (string.IsNullOrEmpty(res.TaskId))
        {
            ExtraLog.LogWithColor("Invalid Pickup & Destination", Color.yellow);
            return;
        }
        if (res != null && !tasks.Any(t => t.TaskId == res.TaskId))
        {
            manager.GUI.Dashboard.AddTask(res);
            tasks.Add(res);
        }
    }

    private IEnumerable<string> GetTaskIds()
    {
        return tasks.Select(t => t.TaskId);
    }

    [Button(ButtonSizes.Medium), TabGroup("Cancel"), GUIColor(1f, 0.4f, 0.4f)]
    private async void CancelTask()
    {
        var task = tasks.Find(t => t.TaskId == cancelTaskId);
        if (task == null) return;
        var res = await manager.WebSocket.SendRequestAsync<DeliveryTask, DeliveryTask>(SocketMessageType.CancelTask, task);
        if (res == null) return;

        var robot = manager.RobotManager.GetRobotByTask(res.TaskId);
        if (robot != null)
        {
            robot.TaskModule.SetTaskStatus(TaskStatus.Cancelled);
        }
        else
        {
            UpdateTaskInfo(res);
        }
    }
    #endregion

    public void UpdateTaskInfo(DeliveryTask newTaskInfo)
    {
        int index = tasks.FindIndex(t => t.TaskId == newTaskInfo.TaskId);

        if (index == -1)
        {
            manager.GUI.Dashboard.AddTask(newTaskInfo);
            tasks.Add(newTaskInfo);
            return;
        }

        manager.GUI.Dashboard.ReplaceTask(newTaskInfo);
        tasks[index] = newTaskInfo;
    }

    public async void CancelTask(DeliveryTask task)
    {
        if (task == null || !tasks.Contains(task)) return;

        var res = await manager.WebSocket.SendRequestAsync<DeliveryTask, DeliveryTask>(SocketMessageType.CancelTask, task);

        if (res == null) return;

        var robot = manager.RobotManager.GetRobotByTask(res.TaskId);
        if (robot != null)
        {
            robot.TaskModule.SetTaskStatus(TaskStatus.Cancelled);
        }
        else
        {
            UpdateTaskInfo(res);
        }
    }

    public async void CreateTask(string pickPoint, string destination, int priority)
    {
        var task = new DeliveryTask
        {
            PickupLocation = pickPoint,
            Destination = destination,
            Priority = priority,
        };

        var res = await manager.WebSocket.SendRequestAsync<DeliveryTask, DeliveryTask>(SocketMessageType.CreateTask, task);
        if (string.IsNullOrEmpty(res.TaskId))
        {
            ExtraLog.LogWithColor("Invalid Pickup & Destination", Color.yellow);
            return;
        }
        if (res != null && !tasks.Any(t => t.TaskId == res.TaskId))
        {
            manager.GUI.Dashboard.AddTask(res);
            tasks.Add(res);
        }
    }
}