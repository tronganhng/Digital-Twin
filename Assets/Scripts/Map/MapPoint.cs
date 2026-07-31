using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class MapPoint : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField] private string pointName;

    public string PointName => pointName;
    public Vector3 Position => transform.position;

    public MapPointDto ToData()
    {
        return new MapPointDto
        {
            PointName = pointName,
            Position = new float[] { Position.x, Position.z }
        };
    }

    [Button]
    private void SetName()
    {
        gameObject.name = pointName;
        label.text = pointName;
    }
}