using Sirenix.OdinInspector;
using UnityEngine;

public class MapLane : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private MapNode startNode;
    [SerializeField] private MapNode endNode;

    [SerializeField, ReadOnly] private float distance;

    public MapLaneDto ToData()
    {
        return new MapLaneDto
        {
            StartNode = startNode.NodeName,
            EndNode = endNode.NodeName,
            Distance = distance,
        };
    }

    [Button]
    private void UpdateLane()
    {
        if (startNode == null || endNode == null)
            return;

        distance = Vector3.Distance(startNode.Position, endNode.Position);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startNode.Position);
        lineRenderer.SetPosition(1, endNode.Position);
    }
}