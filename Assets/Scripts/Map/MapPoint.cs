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
}