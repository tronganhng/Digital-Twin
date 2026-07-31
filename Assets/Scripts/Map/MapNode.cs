using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    [SerializeField] private Collider triggerZone;
    [SerializeField] private TextMeshPro label;
    [SerializeField] private MapPoint pointPrefab;
    [SerializeField] private Transform pointRoot;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private string nodeName;
    [SerializeField, ReadOnly] private List<MapPoint> points;
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
        label.text = nodeName;
        triggerZone.gameObject.SetActive(hasTriggerZone);
        mesh.gameObject.SetActive(hasTriggerZone);

    }

    [Button(ButtonSizes.Medium)]
    private void GetPoint()
    {
        var newPoint = (MapPoint)PrefabUtility.InstantiatePrefab(pointPrefab, pointRoot);
        newPoint.transform.localPosition = Vector3.zero;
        newPoint.transform.localRotation = Quaternion.identity;
        points.Add(newPoint);
    }

    [Button(ButtonSizes.Medium)]
    private void ClearPoints()
    {
        for (int i = pointRoot.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(pointRoot.GetChild(i).gameObject);
        }

        points.Clear();
    }

    public void SetLock(bool isLock)
    {
        Color color = isLock ? occupiedColor : availableColor;
        mesh.material.color = color;
    }

    public MapPoint GetPoint(string pointName)
    {
        return points.FirstOrDefault(p => p.PointName == pointName);
    }
}