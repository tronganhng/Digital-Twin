using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;

public class RobotManager : SimulationBaseService
{
    [Header("References")]
    [SerializeField] private Robot robotPrefab;
    [SerializeField] private Transform robotRoot;

    [SerializeField, ReadOnly] private SerializedDictionary<int, Robot> _robots = new();

    public override void Init(SimulationManager fleet)
    {
        base.Init(fleet);
        
        foreach (var kvp in _robots)
        {
            kvp.Value.Init();
        }
    }

    public Robot GetRobot(int robotIndex)
    {
        if (_robots.TryGetValue(robotIndex, out var robot))
            return robot;

        robot = Instantiate(robotPrefab, Vector3.zero, Quaternion.identity, robotRoot);
        robot.Init();

        _robots.Add(robotIndex, robot);

        return robot;
    }

    private void ClearAllRobots()
    {
        for (int i = robotRoot.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(robotRoot.GetChild(i).gameObject);
        }

        _robots.Clear();
    }

    [Button, PropertySpace(5, 10)]
    private void SpawnRobotBy(GridDrawer grid)
    {
        ClearAllRobots();

        var positions = grid.GetAllPositions();
        if (positions.Count == 0)
            return;

        for (int i = 0; i < positions.Count; i++)
        {
            GetRobot(i).transform.position = positions[i];
        }
    }
}