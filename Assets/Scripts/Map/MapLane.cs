using Sirenix.OdinInspector;
using UnityEngine;

public class MapLane : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private MapPoint startPoint;
    [SerializeField] private MapPoint endPoint;

    [SerializeField, ReadOnly] private float distance;

    public MapLaneDto ToData()
    {
        return new MapLaneDto
        {
            StartPoint = startPoint.PointName,
            EndPoint = endPoint.PointName,
            Distance = distance,
        };
    }

    [Button]
    private void UpdateLane()
    {
        if (startPoint == null || endPoint == null)
            return;

        distance = Vector3.Distance(startPoint.Position, endPoint.Position);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint.Position);
        lineRenderer.SetPosition(1, endPoint.Position);
    }
}