using System;
using System.Threading.Tasks;
using UnityEngine;

public class MoveState : RobotState
{
    private Vector3 _des;
    private Action _onReach;

    protected override RobotStatus Status => RobotStatus.Moving;

    public MoveState(RobotStateMachineModule stateMachine, Vector3 destination, Action onReach = null) : base(stateMachine)
    {
        _des = destination;
        _onReach = onReach;
    }

    public override void Enter()
    {
        base.Enter();
        Robot.MoveModule.MoveTo(_des, _onReach);
        _ = TryReleasePoint();
    }

    private async Task TryReleasePoint()
    {
        var currentPoint = Robot.StatModule.CurrentMapPoint;
        if (currentPoint == null) return;

        var currentNode = currentPoint.GetComponentInParent<MapNode>();

        if (currentNode == null) return;
        var req = new ReleasePointRequest
        {
            NodeName = currentNode.NodeName,
            PointName = currentPoint.PointName
        };

        Robot.StatModule.CurrentMapPoint = null;
        bool isReleased = await SimulationManager.Instance.WebSocket.SendRequestAsync<ReleasePointRequest, bool>(SocketMessageType.ReleaseDockPoint, req);
        if (isReleased)
        {
            currentNode.OnHasFreePoint.Dispatch();
            currentPoint.SetLock(false);
        }
    }
}