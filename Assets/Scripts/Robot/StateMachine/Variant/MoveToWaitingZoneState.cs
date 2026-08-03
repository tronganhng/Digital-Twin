using UnityEngine;
using System;
using System.Threading.Tasks;

/// <summary>
/// Still receive task while moving to waiting zone
/// </summary>
public class MoveToWaitingZoneState : RobotState
{
    protected override RobotStatus Status => RobotStatus.Idle;

    private Vector3 _des;
    private Action _onReach;

    public MoveToWaitingZoneState(RobotStateMachineModule stateMachine, Vector3 destination, Action onReach = null) : base(stateMachine)
    {
        _des = destination;
        _onReach = onReach;
    }

    public override void Enter()
    {
        base.Enter();
        Robot.MoveModule.MoveTo(_des, _onReach);
        _ = TryReleasePoint();
        Robot.TaskModule.OnTaskStart.AddOnce(OnTaskStart);
    }

    public override void Exit()
    {
        base.Exit();
        Robot.TaskModule.OnTaskStart.RemoveOnce(OnTaskStart);
    }

    private void OnTaskStart()
    {
        Robot.TaskModule.SetTaskStatus(TaskStatus.Running);
        var pickupNode = Robot.TaskModule.PickupNode;
        StateMachine.ChangeState(new MoveState(StateMachine, pickupNode.Position, () =>
            StateMachine.ChangeState(new AcquireDockPointState(StateMachine, pickupNode, () =>
                StateMachine.ChangeState(new WaitingState(StateMachine))))));
    }

    private async Task TryReleasePoint()
    {
        var currentPoint = Robot.StatModule.CurrentMapPoint;
        if (currentPoint == null) return;

        var currentNode = currentPoint.Node;

        if (currentNode == null) return;
        var req = new ReleasePointRequest
        {
            NodeName = currentNode.NodeName,
            PointName = currentPoint.PointName
        };

        bool isReleased = await SimulationManager.Instance.WebSocket.SendRequestAsync<ReleasePointRequest, bool>(SocketMessageType.ReleaseDockPoint, req);
        if (isReleased)
        {
            currentNode.OnHasFreePoint.Dispatch();
            currentPoint.SetLock(false);
            Robot.StatModule.CurrentMapPoint = null;
        }
    }
}