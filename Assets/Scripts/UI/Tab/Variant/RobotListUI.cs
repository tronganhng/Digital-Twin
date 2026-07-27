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
}