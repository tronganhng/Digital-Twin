using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class MapManager : SimulationBaseService
{
    [SerializeField] private Transform mapRoot;

    [SerializeField, ReadOnly] private SerializedDictionary<string, MapPoint> _points = new();

    public override void Init(SimulationManager fleetManager)
    {
        base.Init(fleetManager);
        CachePoints();
    }

    [Button]
    private void CachePoints()
    {
        _points.Clear();

        MapPoint[] points = mapRoot.GetComponentsInChildren<MapPoint>(true);

        foreach (MapPoint point in points)
        {
            if (_points.ContainsKey(point.PointName))
            {
                Debug.LogWarning($"Duplicate MapPoint: {point.PointName}");
                continue;
            }

            _points.Add(point.PointName, point);
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
            Points = _points.Values.Select(p => p.ToData()).ToList()
        });
    }
}