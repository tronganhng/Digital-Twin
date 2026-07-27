using System;
using Sigtrap.Relays;
using UnityEngine;

[Serializable]
public class RobotStateDto
{
    [field: SerializeField] public string RobotId { get; set; } = string.Empty;
    [field: SerializeField] public double X { get; set; }
    [field: SerializeField] public double Y { get; set; }
    [field: SerializeField] public double Rotation { get; set; }
    [field: SerializeField] public double Battery { get; set; }
    [field: SerializeField] public RobotStatus Status { get; set; }
    [field: SerializeField] public string CurrentTaskId { get; set; }
    [field: SerializeField] public DateTime LastHeartbeat { get; set; }
}