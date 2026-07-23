using UnityEngine;

public class TaskListUI : TabContent
{
    [SerializeField] private TaskItemUI itemPrefab;
    [SerializeField] private Transform container;

    public void AddTaskItem()
    {
        Instantiate(itemPrefab, container);
    }
}