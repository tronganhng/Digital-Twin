using Sirenix.OdinInspector;
using UnityEngine;

public class ChargingPole : MonoBehaviour
{
    [SerializeField] private Transform chargePoint;

    [field: SerializeField, ReadOnly] public bool IsOccupied { get; private set; }
}