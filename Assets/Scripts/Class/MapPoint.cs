using UnityEngine;

public class MapPoint : MonoBehaviour
{
    [SerializeField] private string pointName;

    public string PointName => pointName;
    public Vector3 Position => transform.position;
}