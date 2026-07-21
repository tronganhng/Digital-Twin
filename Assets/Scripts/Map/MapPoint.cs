using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField] private string pointName;

    public string PointName => pointName;
    public Vector3 Position => transform.position;

    void Start()
    {
        label.text = pointName;
    }

    public MapPointDto ToData()
    {
        return new MapPointDto
        {
            PointName = pointName,
            Position = new float[] { Position.x, Position.z }
        };
    }

    public void FromData(MapPointDto data)
    {
        var pos = new Vector3(data.Position[0], 0, data.Position[1]);
        pointName = data.PointName;
        transform.position = pos;
    }

    [Button]
    private void SetName()
    {
        gameObject.name = pointName;
        label.text = pointName;
    }
}