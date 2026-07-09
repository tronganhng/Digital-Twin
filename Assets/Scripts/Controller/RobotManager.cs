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

    public IReadOnlyDictionary<int, Robot> GetAllRobots()
    {
        return _robots;
    }
}