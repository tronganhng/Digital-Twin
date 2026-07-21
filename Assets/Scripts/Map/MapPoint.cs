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
            pointName = pointName,
            position = new float[] { Position.x, Position.z }
        };
    }

    public void FromData(MapPointDto data)
    {
        var pos = new Vector3(data.position[0], 0, data.position[1]);
        pointName = data.pointName;
        transform.position = pos;
    }
}