using System;
using Sigtrap.Relays;
using UnityEngine;

[Serializable]
public class ElevatorDto
{
    [field: SerializeField] public string ElevatorId { get; set; } = string.Empty;
    [field: SerializeField] public int CurrentFloor { get; set; } = 1;
    [field: SerializeField] public ElevatorStatus Status { get; set; }
}