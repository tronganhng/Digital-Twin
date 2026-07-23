using Sirenix.OdinInspector;
using UnityEngine;

public class ChargingPole : MonoBehaviour
{
    [SerializeField] private Transform chargePoint;
    [SerializeField] private MeshRenderer[] lightningMeshs;

    [SerializeField] private Color availableColor = Color.green;
    [SerializeField] private Color occupiedColor = Color.red;

    [SerializeField, ReadOnly]
    private bool isOccupied;

    public bool IsOccupied
    {
        get => isOccupied;
        set
        {
            if (isOccupied == value)
                return;

            isOccupied = value;
            UpdateColor();
        }
    }

    public Transform ChargePoint => chargePoint;

    private void Awake()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        Color color = isOccupied ? occupiedColor : availableColor;

        foreach (var mesh in lightningMeshs)
        {
            if (mesh == null)
                continue;

            mesh.material.color = color;
        }
    }
}