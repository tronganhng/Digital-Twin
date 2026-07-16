using System;
using UnityEngine;

[Serializable]
public class DeliveryTask
{
    [field : SerializeField] public string TaskId { get; set; } = string.Empty;
    [field : SerializeField] public string PickupLocation { get; set; } = string.Empty;
    [field : SerializeField] public string Destination { get; set; } = string.Empty;
    [field : SerializeField] public int Priority { get; set; }
    [field : SerializeField] public string AssignedRobotId { get; set; }
    [field : SerializeField] public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public void CopyFrom(DeliveryTask other)
    {
        Status = other.Status;
        AssignedRobotId = other.AssignedRobotId;
        Priority = other.Priority;
        PickupLocation = other.PickupLocation;
        Destination = other.Destination;
    }
}