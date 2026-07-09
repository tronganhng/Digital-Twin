using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;

public class RobotManager : FleetBaseModule
{
    [Header("References")]
    [SerializeField] private Robot robotPrefab;
    [SerializeField] private Transform robotRoot;

    [SerializeField, ReadOnly] private SerializedDictionary<int, Robot> _robots = new();

    public Robot GetRobot(int robotId)
    {
        if (_robots.TryGetValue(robotId, out var robot))
            return robot;

        robot = Instantiate(robotPrefab, Vector3.zero, Quaternion.identity, robotRoot);
        robot.Init(robotId);

        _robots.Add(robotId, robot);

        return robot;
    }

    public void RemoveRobot(int robotId)
    {
        if (!_robots.TryGetValue(robotId, out var robot))
            return;

        Destroy(robot.gameObject);
        _robots.Remove(robotId);
    }

    private void ClearAllRobots()
    {
        for (int i = robotRoot.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(robotRoot.GetChild(i).gameObject);
        }

        _robots.Clear();
    }

    public IReadOnlyDictionary<int, Robot> GetAllRobots()
    {
        return _robots;
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