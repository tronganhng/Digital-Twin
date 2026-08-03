using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class MapPoint : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Color availableColor = Color.turquoise;
    [SerializeField] private Color occupiedColor = Color.red;
    [SerializeField] private string pointName;

    public string PointName => pointName;
    public Vector3 Position => transform.position;
    public MapNode Node { get; private set; }

    void Start()
    {
        Node = GetComponentInParent<MapNode>();
    }

    public MapPointDto ToData()
    {
        return new MapPointDto
        {
            PointName = pointName,
            Position = new float[] { Position.x, Position.z }
        };
    }

    public void SetLock(bool isLock)
    {
        Color color = isLock ? occupiedColor : availableColor;
        mesh.material.color = color;
        label.color = color;
    }

    [Button]
    private void SetName()
    {
        gameObject.name = pointName;
        label.text = pointName;
    }
}