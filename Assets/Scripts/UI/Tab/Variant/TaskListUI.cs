using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TaskListUI : TabContent
{
    [SerializeField] private TaskItemUI itemPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private TMP_Dropdown priorityDrop, startPointDrop, endPointDrop;

    private List<TaskItemUI> _taskItems = new();

    public void Init()
    {
        var pointNames = SimulationManager.Instance.MapManager.GetPointNames().ToList();

        startPointDrop.ClearOptions();
        endPointDrop.ClearOptions();
        priorityDrop.ClearOptions();

        priorityDrop.AddOptions(new List<string> { "0", "1", "2", "3" });
        startPointDrop.AddOptions(pointNames);
        endPointDrop.AddOptions(pointNames);

        if (pointNames.Count > 0)
        {
            startPointDrop.value = 0;
            endPointDrop.value = Mathf.Min(1, pointNames.Count - 1);

            startPointDrop.RefreshShownValue();
            endPointDrop.RefreshShownValue();
        }
    }

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

    public void CreateTask()
    {
        if (startPointDrop.options.Count == 0 || endPointDrop.options.Count == 0)
            return;

        var pickupPoint = startPointDrop.options[startPointDrop.value].text;
        var destination = endPointDrop.options[endPointDrop.value].text;

        if (pickupPoint == destination)
        {
            Debug.LogWarning("Pickup point and destination cannot be the same.");
            return;
        }

        if (!int.TryParse(priorityDrop.options[priorityDrop.value].text, out int priority))
        {
            Debug.LogWarning("Invalid priority.");
            return;
        }

        SimulationManager.Instance.TaskManager.CreateTask(pickupPoint, destination, priority);
    }
}