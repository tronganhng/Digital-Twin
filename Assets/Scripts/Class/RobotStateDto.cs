using UnityEngine;

public class RobotStateDto
{
    public int robotId;

    public Vector3 position;

    public Quaternion rotation;

    public float battery;

    public RobotStatus status;

    public string currentTask;
}