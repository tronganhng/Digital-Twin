using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RobotManager : SimulationBaseService
{
    [Header("References")]
    [SerializeField] private Robot robotPrefab;
    [SerializeField] private Transform robotRoot;

    [SerializeField, ReadOnly] private List<Robot> _robots = new();

    public override void Init(SimulationManager fleet)
    {
        base.Init(fleet);

        foreach (var robot in _robots)
        {
            robot.Init();
        }
    }

    public Robot GetRobotBy(string robotId)
    {
        foreach (var robot in _robots)
        {
            if (robot.StatModule.RobotId == robotId) return robot;
        }
        return null;
    }

    private void ClearAllRobots()
    {
        for (int i = robotRoot.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(robotRoot.GetChild(i).gameObject);
        }

        _robots.Clear();
    }

    private Robot GetRobot()
    {
        Robot robot = null;
#if UNITY_EDITOR
        robot = (Robot)PrefabUtility.InstantiatePrefab(robotPrefab, robotRoot);
        robot.transform.localPosition = Vector3.zero;
        robot.transform.localRotation = Quaternion.identity;
        _robots.Add(robot);
#endif
        return robot;
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
            GetRobot().transform.position = positions[i];
        }
    }
}