using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class MapManager : FleetBaseModule
{
    [SerializeField] private Transform mapRoot;

    [SerializeField, ReadOnly] private SerializedDictionary<string, MapPoint> _points = new();

    public override void Init(FleetManager fleetManager)
    {
        base.Init(fleetManager);
        CachePoints();
    }

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

    public MapPoint GetPoint(string pointName)
    {
        _points.TryGetValue(pointName, out MapPoint point);
        return point;
    }
}