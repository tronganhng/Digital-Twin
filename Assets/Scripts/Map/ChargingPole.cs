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

    [SerializeField, ReadOnly]
    private Robot owner;

    public bool IsOccupied => isOccupied;
    public Robot Owner => owner;
    public Transform ChargePoint => chargePoint;

    private void Awake()
    {
        UpdateColor();
    }

    /// <summary>
    /// Chiếm trạm sạc.
    /// </summary>
    public bool Occupy(Robot robot)
    {
        if (robot == null)
            return false;

        if (isOccupied)
            return owner == robot;

        owner = robot;
        isOccupied = true;

        owner.StatModule.OnRobotOffline.AddOnce(OnOwnerOffline);

        UpdateColor();

        return true;
    }

    /// <summary>
    /// Giải phóng trạm sạc.
    /// </summary>
    public void Release()
    {
        if (!isOccupied)
            return;

        if (owner != null)
        {
            owner.StatModule.OnRobotOffline.RemoveOnce(OnOwnerOffline);
        }

        owner = null;
        isOccupied = false;

        UpdateColor();
    }

    private void OnOwnerOffline()
    {
        Release();
    }

    private void UpdateColor()
    {
        var color = isOccupied ? occupiedColor : availableColor;

        foreach (var mesh in lightningMeshs)
        {
            if (mesh == null)
                continue;

            mesh.material.color = color;
        }
    }
}