using System.Collections.Generic;
using UnityEngine;

public class HospitalMapManager : MonoSingleton<HospitalMapManager>
{
    private Dictionary<string, HospitalPoint> pointLookup = new();

    protected override void Awake()
    {
        base.Awake();

        HospitalPoint[] points = FindObjectsByType<HospitalPoint>(FindObjectsSortMode.None);

        foreach (HospitalPoint point in points)
        {
            pointLookup[point.id] = point;
        }
    }

    public HospitalPoint GetPoint(string id)
    {
        pointLookup.TryGetValue(id, out HospitalPoint point);
        return point;
    }

    public Vector3 GetPosition(string id)
    {
        return GetPoint(id).transform.position;
    }

    public IEnumerable<HospitalPoint> GetAllPoints()
    {
        return pointLookup.Values;
    }
}