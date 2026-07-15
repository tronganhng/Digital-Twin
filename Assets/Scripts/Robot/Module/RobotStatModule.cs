using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class RobotStatModule : RobotBaseModule
{
    [SerializeField] private TextMeshPro statusTmp;
    [SerializeField, ReadOnly] private int robotId;
    [SerializeField, ReadOnly] private RobotStatus status = RobotStatus.Idle;
    [SerializeField, ReadOnly] private float battery = 100;

    public int RobotId { get { return robotId; } set { robotId = value; } }
    public RobotStatus Status { get { return status; } set { status = value; statusTmp.text = $"{value}"; } }
    public float Battery { get { return battery; } set { battery = value; } }

    public void Init(Robot robot, int robotId)
    {
        base.Init(robot);
        this.robotId = robotId;
        Status = RobotStatus.Idle;
        Battery = 100;
    }

    public bool IsAvailableForTask()
    {
        return status == RobotStatus.Idle && battery > 20f;
    }
}