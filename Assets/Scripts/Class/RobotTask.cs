
using System;

[Serializable]
public class RobotTask
{
    public int Id;

    public string PickupPoint;

    public string DestinationPoint;

    public int? AssignedRobotId;

    public TaskStatus Status;

    public float CreateTime;
}