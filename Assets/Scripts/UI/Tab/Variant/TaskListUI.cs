using UnityEngine;

public class TaskListUI : TabContent
{
    [SerializeField] private TaskItemUI itemPrefab;
    [SerializeField] private Transform container;

    public void AddTaskItem(DeliveryTask task)
    {
        var newTask = Instantiate(itemPrefab, container);
        newTask.Init(task);
    }
}