using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class MapManager : SimulationBaseService
{
    [SerializeField] private ChargingStation chargingStation;
    [SerializeField] private Transform mapRoot;

    [SerializeField, ReadOnly] private SerializedDictionary<string, MapNode> _nodes = new();
    [SerializeField, ReadOnly] private List<MapLane> _lanes = new();

    public override void Init(SimulationManager fleetManager)
    {
        base.Init(fleetManager);
        SetupData();
    }

    [Button(ButtonSizes.Large)]
    private void SetupData()
    {
        _nodes.Clear();
        _lanes.Clear();

        MapNode[] points = mapRoot.GetComponentsInChildren<MapNode>(true);
        MapLane[] lanes = mapRoot.GetComponentsInChildren<MapLane>(true);

        foreach (MapNode point in points)
        {
            if (_nodes.ContainsKey(point.NodeName))
            {
                Debug.LogWarning($"Duplicate MapPoint: {point.NodeName}");
                continue;
            }

            _nodes.Add(point.NodeName, point);
        }

        foreach (var lane in lanes)
        {
            _lanes.Add(lane);
        }
    }

    public bool TryGetNode(string nodeName, out MapNode position)
    {
        if (_nodes.TryGetValue(nodeName, out MapNode point))
        {
            position = point;
            return true;
        }

        position = default;
        return false;
    }

    public ChargingPole GetFreeChargingPole(Robot owner) => chargingStation.GetFreePole(owner);

    [Button(ButtonSizes.Large)]
    private void Save()
    {
        SaveLoad.Save("Map", new MapDto
        {
            Nodes = _nodes.Values.Select(p => p.ToData()).ToList(),
            Lanes = _lanes.Select(p => p.ToData()).ToList()
        });
    }

    public IEnumerable<string> GetNodeNames()
    {
        return _nodes.Keys;
    }
}