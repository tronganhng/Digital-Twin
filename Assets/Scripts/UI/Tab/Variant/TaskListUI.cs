using System.Collections.Generic;
using UnityEngine;

public class TaskListUI : TabContent
{
    [SerializeField] private TaskItemUI itemPrefab;
    [SerializeField] private Transform container;

    private List<TaskItemUI> _taskItems = new();

    public void AddTaskItem(DeliveryTask task)
    {
        var newTask = Instantiate(itemPrefab, container);
        newTask.Init(task);
        _taskItems.Add(newTask);
    }

    public void ReplaceTaskItem(DeliveryTask task)
    {
        var targetItem = _taskItems.Find(t => t.TaskId == task.TaskId);

        if (!targetItem) return;

        targetItem.Init(task);
    }
}