using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine;
using Sirenix.OdinInspector;

public class TaskManager : FleetBaseModule
{
    [SerializeField, ReadOnly] private SerializedDictionary<int, RobotTask> _tasks = new();

    private readonly Queue<RobotTask> _pendingTasks = new();

    private int _nextTaskId = 1;

    #region Create

    public RobotTask CreateTask(string pickupPoint, string destinationPoint)
    {
        RobotTask task = new RobotTask
        {
            Id = _nextTaskId++,
            PickupPoint = pickupPoint,
            DestinationPoint = destinationPoint,
            Status = TaskStatus.Pending,
            CreateTime = Time.time
        };

        _tasks.Add(task.Id, task);
        _pendingTasks.Enqueue(task);

        Debug.Log($"Create Task {task.Id}");

        return task;
    }

    #endregion

    #region Scheduler

    public bool TryGetNextPendingTask(out RobotTask task)
    {
        while (_pendingTasks.Count > 0)
        {
            task = _pendingTasks.Dequeue();

            if (task.Status == TaskStatus.Pending)
                return true;
        }

        task = null;
        return false;
    }

    #endregion

    #region Update

    public void AssignTask(int taskId, int robotId)
    {
        if (!_tasks.TryGetValue(taskId, out RobotTask task))
            return;

        task.AssignedRobotId = robotId;
        task.Status = TaskStatus.Assigned;
    }

    public void StartTask(int taskId)
    {
        if (_tasks.TryGetValue(taskId, out RobotTask task))
            task.Status = TaskStatus.Running;
    }

    public void CompleteTask(int taskId)
    {
        if (_tasks.TryGetValue(taskId, out RobotTask task))
            task.Status = TaskStatus.Completed;
    }

    public void FailTask(int taskId)
    {
        if (_tasks.TryGetValue(taskId, out RobotTask task))
            task.Status = TaskStatus.Failed;
    }

    public void CancelTask(int taskId)
    {
        if (_tasks.TryGetValue(taskId, out RobotTask task))
            task.Status = TaskStatus.Cancelled;
    }

    #endregion

    #region Query

    public RobotTask GetTask(int taskId)
    {
        _tasks.TryGetValue(taskId, out RobotTask task);
        return task;
    }

    public List<RobotTask> GetTasks(TaskStatus status)
    {
        return _tasks.Values
                     .Where(t => t.Status == status)
                     .ToList();
    }

    public IReadOnlyCollection<RobotTask> GetAllTasks()
    {
        return _tasks.Values;
    }

    #endregion
}