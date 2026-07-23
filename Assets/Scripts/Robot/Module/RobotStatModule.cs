using System;
using System.Collections;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class RobotStatModule : RobotBaseModule
{
    [SerializeField] private TextMeshPro idTmp, statusTmp;
    [SerializeField] private float sendDataInterval = 0.5f;

    [SerializeField, ReadOnly] private RobotStateDto _robotState;

    private bool _disable;
    private Coroutine _sendDataCrt;

    public string RobotId => _robotState.RobotId;
    public double Battery { get { return _robotState.Battery; } set { _robotState.Battery = value; } }
    public bool IsRegistered { get; private set; }

    public async Task InitStat(Robot robot)
    {
        base.Init(robot);

        _robotState = new RobotStateDto
        {
            Battery = 100,
            Status = RobotStatus.Idle
        };

        await TryRegister();
        _sendDataCrt = StartCoroutine(SendDataCrt());
    }

    private void UpdateState()
    {
        _robotState.X = robot.transform.position.x;
        _robotState.Y = robot.transform.position.z;
        _robotState.Rotation = robot.transform.eulerAngles.y;
        _robotState.LastHeartbeat = DateTime.UtcNow;
    }

    private async Task TryRegister()
    {
        UpdateState();

        var res = await SimulationManager.Instance.WebSocket.SendRequestAsync<RobotStateDto, RobotStateDto>(SocketMessageType.RegisterRobot, _robotState);
        _robotState.RobotId = res.RobotId;
        idTmp.text = res.RobotId;
        IsRegistered = true;

    }

    private async Task SendStateAsync()
    {
        if (!IsRegistered || _disable)
            return;

        UpdateState();

        await SimulationManager.Instance.WebSocket.SendMessageAsync(SocketMessageType.RobotState, _robotState);
    }

    public void SetStatus(RobotStatus status)
    {
        statusTmp.text = status.ToString();
        _robotState.Status = status;
        UpdateState();
        _ = SendStateAsync();
    }

    public void SetTaskId(string id)
    {
        _robotState.CurrentTaskId = id;
    }

    private IEnumerator SendDataCrt()
    {
        while (true)
        {
            UpdateState();
            _ = SendStateAsync();
            yield return new WaitForSeconds(sendDataInterval);
        }
    }

    public void Disable()
    {
        _disable = true;
        StopCoroutine(_sendDataCrt);
    }
}