using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RobotStatModule : RobotBaseModule
{
    [SerializeField] private TextMeshPro statusTmp;

    private RobotStateDto _robotState;

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
    }

    private void UpdateState()
    {
        _robotState.X = robot.transform.position.x;
        _robotState.Y = robot.transform.position.z;
        _robotState.Rotation = robot.transform.eulerAngles.y;
        _robotState.Battery = 100;
        _robotState.LastHeartbeat = DateTime.UtcNow;
    }

    public async Task SendStateAsync()
    {
        if (!IsRegistered)
            return;

        UpdateState();

        await SimulationManager.Instance.WebSocket.SendMessageAsync(SocketMessageType.RobotState, _robotState);
    }
}