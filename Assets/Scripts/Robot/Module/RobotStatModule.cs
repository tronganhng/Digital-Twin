using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class RobotStatModule : RobotBaseModule
{
    [SerializeField] private TextMeshPro statusTmp;

    [SerializeField, ReadOnly] private RobotStateDto _robotState;

    public string RobotId => _robotState.RobotId;
    public bool IsRegistered { get; private set; }

    public override void Init(Robot robot)
    {
        base.Init(robot);

        _robotState = new RobotStateDto
        {
            Battery = 100,
            Status = RobotStatus.Idle
        };

        _ = TryRegister();
    }

    private void UpdateState()
    {
        _robotState.X = robot.transform.position.x;
        _robotState.Y = robot.transform.position.z;
        _robotState.Rotation = robot.transform.eulerAngles.y;
        _robotState.Battery = 100;
        _robotState.LastHeartbeat = DateTime.UtcNow;
    }

    public async Task TryRegister()
    {
        UpdateState();

        var res = await SimulationManager.Instance.WebSocket.SendRequestAsync<RobotStateDto, RobotStateDto>(SocketMessageType.RegisterRobot, _robotState);
        _robotState.RobotId = res.RobotId;
        IsRegistered = true;
    }

    public async Task SendStateAsync()
    {
        if (!IsRegistered)
            return;

        UpdateState();

        await SimulationManager.Instance.WebSocket.SendMessageAsync(SocketMessageType.RobotState, _robotState);
    }

    public void SetTaskId(string id)
    {
        _robotState.CurrentTaskId = id;
    }
}