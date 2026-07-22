using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class MapManager : SimulationBaseService
{
    [SerializeField] private ChargingStation chargingStation;
    [SerializeField] private Transform mapRoot;

    [SerializeField, ReadOnly] private SerializedDictionary<string, MapPoint> _points = new();
    [SerializeField, ReadOnly] private List<MapLane> _lanes = new();

    public override void Init(SimulationManager fleetManager)
    {
        base.Init(fleetManager);
        SetupData();
    }

    [Button]
    private void SetupData()
    {
        _points.Clear();
        _lanes.Clear();

        MapPoint[] points = mapRoot.GetComponentsInChildren<MapPoint>(true);
        MapLane[] lanes = mapRoot.GetComponentsInChildren<MapLane>(true);

        foreach (MapPoint point in points)
        {
            if (_points.ContainsKey(point.PointName))
            {
                Debug.LogWarning($"Duplicate MapPoint: {point.PointName}");
                continue;
            }

            _points.Add(point.PointName, point);
        }

        foreach (var lane in lanes)
        {
            _lanes.Add(lane);
        }
    }

    public bool TryGetPoint(string pointName, out MapPoint position)
    {
        if (_points.TryGetValue(pointName, out MapPoint point))
        {
            position = point;
            return true;
        }

        position = default;
        return false;
    }

    [Button]
    private void Save()
    {
        SaveLoad.Save("Map", new MapDto
        {
            Points = _points.Values.Select(p => p.ToData()).ToList(),
            Lanes = _lanes.Select(p => p.ToData()).ToList()
        });
    }

    public IEnumerable<string> GetPointNames()
    {
        return _points.Keys;
    }
}