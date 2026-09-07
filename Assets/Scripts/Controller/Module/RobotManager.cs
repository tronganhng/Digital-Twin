using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RobotManager : SimulationBaseService
{
    [Header("References")]
    [SerializeField] private RobotRoot robotRootPrefab;
    [SerializeField] private Transform robotRoot;

    [SerializeField, ReadOnly] private List<Robot> _robots = new();

    public async Task InitRobot(SimulationManager fleet)
    {
        base.Init(fleet);

        foreach (var robot in _robots)
        {
            await robot.Init();
        }
    }

    public Robot GetRobotBy(string robotId)
    {
        foreach (var robot in _robots)
        {
            if (robot.StatModule.StateDto.RobotId == robotId) return robot;
        }
        var newRobotRoot = Instantiate(robotRootPrefab, robotRoot);
        _robots.Add(newRobotRoot.Robot);
        return newRobotRoot.Robot;
    }

    public List<Robot> GetAllRobot()
    {
        return _robots;
    }

    public void ClearAllRobots()
    {
        for (int i = robotRoot.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(robotRoot.GetChild(i).gameObject);
        }

        _robots.Clear();
    }

    private RobotRoot GetRobotRoot()
    {
        RobotRoot robotRoot = null;
#if UNITY_EDITOR
        robotRoot = (RobotRoot)PrefabUtility.InstantiatePrefab(robotRootPrefab, this.robotRoot);
        robotRoot.transform.localPosition = Vector3.zero;
        robotRoot.transform.localRotation = Quaternion.identity;
        _robots.Add(robotRoot.Robot);
#endif
        return robotRoot;
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
            GetRobotRoot().transform.position = positions[i];
        }
    }
}