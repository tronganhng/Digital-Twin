using UnityEngine;

public class MapPoint : MonoBehaviour
{
    [SerializeField] private string pointName;
    public Vector3 Position => transform.position;
    
    public MapPointDto ToData()
    {
        return new MapPointDto
        {
            PointName = pointName,
            Position = new float[] { Position.x, Position.z }
        };
    }
}