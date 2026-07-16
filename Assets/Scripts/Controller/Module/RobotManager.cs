using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
        robot = (Robot)PrefabUtility.InstantiatePrefab(robotPrefab, robotRoot);
        robot.transform.localPosition = Vector3.zero;
        robot.transform.localRotation = Quaternion.identity;
#else
    robot = Instantiate(robotPrefab, Vector3.zero, Quaternion.identity, robotRoot);
#endif

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