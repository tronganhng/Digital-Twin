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
        var pointDto = await SimulationManager.Instance.WebSocket.SendRequestAsync<string, MapPointDto>(SocketMessageType.AcquireDockPoint, nodeDto.NodeName);

        if (pointDto.PointName == string.Empty)
        {
            _targetNode.OnHasFreePoint.AddOnce(() => _ = SendReq());
        }
        else
        {
            var targetPoint = _targetNode.GetPoint(pointDto.PointName);
            StateMachine.ChangeState(new MoveState(StateMachine, targetPoint.Position, () =>
            {
                _onAcquired?.Invoke();
                Robot.StatModule.CurrentMapPoint = targetPoint;
            }));
        }
    }
}