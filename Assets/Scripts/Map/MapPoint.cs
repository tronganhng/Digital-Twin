using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
    [SerializeField] private Collider triggerZone;
    [SerializeField] private TextMeshPro label;
    [SerializeField] private string pointName;
    [SerializeField] private MeshRenderer[] pointMeshs;
    [SerializeField] private bool hasTriggerZone;
    [SerializeField, ShowIf(nameof(hasTriggerZone))] private Color availableColor = Color.green;
    [SerializeField, ShowIf(nameof(hasTriggerZone))] private Color occupiedColor = Color.red;

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
        triggerZone.gameObject.SetActive(hasTriggerZone);
        foreach (var item in pointMeshs)
        {
            item.gameObject.SetActive(hasTriggerZone);
        }
    }

    public void SetLock(bool isLock)
    {
        Color color = isLock ? occupiedColor : availableColor;

        foreach (var mesh in pointMeshs)
        {
            if (mesh == null)
                continue;

            mesh.material.color = color;
        }
    }
}