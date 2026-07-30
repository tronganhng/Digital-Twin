using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    [SerializeField] private Collider triggerZone;
    [SerializeField] private TextMeshPro label;
    [SerializeField] private string nodeName;
    [SerializeField] private List<MeshRenderer> meshs;
    [SerializeField] private List<MapPoint> points;
    [SerializeField] private bool hasTriggerZone;
    [SerializeField, ShowIf(nameof(hasTriggerZone))] private Color availableColor = Color.green;
    [SerializeField, ShowIf(nameof(hasTriggerZone))] private Color occupiedColor = Color.red;

    public string NodeName => nodeName;
    public Vector3 Position => transform.position;

    void Start()
    {
        label.text = nodeName;
    }

    public MapNodeDto ToData()
    {
        return new MapNodeDto
        {
            NodeName = nodeName,
            Position = new float[] { Position.x, Position.z },
            MapPoints = points.Select(p => p.ToData()).ToList()
        };
    }

    [Button]
    private void SetName()
    {
        gameObject.name = $"node_{nodeName}";
        label.text = $"node_{nodeName}";
        triggerZone.gameObject.SetActive(hasTriggerZone);
        foreach (var item in meshs)
        {
            item.gameObject.SetActive(hasTriggerZone);
        }
    }

    public void SetLock(bool isLock)
    {
        Color color = isLock ? occupiedColor : availableColor;

        foreach (var mesh in meshs)
        {
            if (mesh == null)
                continue;

            mesh.material.color = color;
        }
    }
}