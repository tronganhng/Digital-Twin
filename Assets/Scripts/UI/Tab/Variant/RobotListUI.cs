using UnityEngine;

public class RobotListUI : TabContent
{
    [SerializeField] private RobotItemUI itemPrefab;
    [SerializeField] private Transform container;

    public void Init()
    {
        var robots = SimulationManager.Instance.RobotManager.GetAllRobot();

        foreach (var robot in robots)
        {
            var itemUI = Instantiate(itemPrefab, container);
            itemUI.Init(robot);
        }
    }

    public void ClearAll()
    {
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(container.GetChild(i).gameObject);
        }
    }
}