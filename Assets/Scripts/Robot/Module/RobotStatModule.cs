using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RobotStatModule : RobotBaseModule
{
    [SerializeField] private TextMeshPro statusTmp;

    private RobotStateDto _robotState;

    public string RobotId { get { return _robotState.RobotId; } private set { _robotState.RobotId = value; } }

    public bool IsRegistered { get; private set; }

    public override void Init(Robot robot)
    {
        base.Init(robot);

        _robotState = new RobotStateDto
        {
            Battery = 100,
            Status = RobotStatus.Idle
        };

        UpdateState();

        // Bắt đầu quá trình đăng ký nhưng không block Unity
        _ = RegisterAsync();
    }

    private void UpdateState()
    {
        _robotState.X = robot.transform.position.x;
        _robotState.Y = robot.transform.position.z;
        _robotState.Rotation = robot.transform.eulerAngles.y;
        _robotState.Battery = 100;
        _robotState.LastHeartbeat = DateTime.UtcNow;
    }

    private async Task RegisterAsync()
    {
        try
        {
            statusTmp.text = "Registering...";

            UpdateState();

            RegisterRobotResponse response = await SimulationManager.Instance.WebSocket.RegisterRobotAsync(_robotState);

            RobotId = response.RobotId;

            IsRegistered = true;

            statusTmp.text = $"Online ({RobotId})";

            OnRegistered();
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            statusTmp.text = "Register Failed";
        }
    }

    private void OnRegistered()
    {
        // Thông báo cho Robot rằng đã được backend chấp nhận
    }

    public async Task SendStateAsync()
    {
        if (!IsRegistered)
            return;

        UpdateState();

        await SimulationManager.Instance.WebSocket.SendRobotStateAsync(_robotState);
    }
}