using System.Threading.Tasks;
using UnityEngine;
using System;

public class AcquireDockPointState : RobotState
{
    protected override RobotStatus Status => RobotStatus.Moving;

    private MapNode _targetNode;
    private Action _onAcquired;

    public AcquireDockPointState(RobotStateMachineModule stateMachine, MapNode targetNode, Action onAcquired) : base(stateMachine)
    {
        _targetNode = targetNode;
        _onAcquired = onAcquired;
    }

    public override void Enter()
    {
        base.Enter();
        _ = SendReq();
    }

    private async Task SendReq()
    {
        var nodeDto = _targetNode.ToData();
        var targetPoint = await SimulationManager.Instance.WebSocket.SendRequestAsync<string, MapPointDto>(SocketMessageType.AcquireDockPoint, nodeDto.NodeName);
        var des = new Vector3(targetPoint.Position[0], 0, targetPoint.Position[1]);
        StateMachine.ChangeState(new MoveState(StateMachine, des, _onAcquired));
    }
}