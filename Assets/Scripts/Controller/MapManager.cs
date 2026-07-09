using System.Collections.Generic;
using UnityEngine;

public class MapManager : FleetBaseModule
{
    [SerializeField] private Transform mapRoot;

    private readonly Dictionary<string, MapPoint> _points = new();

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

    public Vector3 GetPoint(string pointName)
    {
        if (_points.TryGetValue(pointName, out MapPoint point))
            return point.Position;

        Debug.LogError($"MapPoint '{pointName}' not found.");

        return Vector3.zero;
    }

    public bool TryGetPoint(string pointName, out Vector3 position)
    {
        if (_points.TryGetValue(pointName, out MapPoint point))
        {
            position = point.Position;
            return true;
        }

        position = default;
        return false;
    }

    public MapPoint GetMapPoint(string pointName)
    {
        _points.TryGetValue(pointName, out MapPoint point);
        return point;
    }
}

public class MapPoint : MonoBehaviour
{
    [SerializeField] private string pointName;

    public string PointName => pointName;
    public Vector3 Position => transform.position;
}