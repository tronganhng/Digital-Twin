using UnityEngine;

public class Dashboard : MonoBehaviour
{
    [SerializeField] private Tabs tabs;
    [SerializeField] private TaskListUI taskListUI;
    [SerializeField] private RobotListUI robotListUI;

    public void Init()
    {
        tabs.Init();
        taskListUI.Init();
        robotListUI.Init();
        tabs.SetTab(0, true);
    }

    public void AddTask(DeliveryTask task) => taskListUI.AddTaskItem(task);

    public void ClearAllTask() => taskListUI.ClearAll();

    public void ReplaceTask(DeliveryTask task) => taskListUI.ReplaceTaskItem(task);
}