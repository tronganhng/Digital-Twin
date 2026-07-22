using UnityEngine;

public class IdleState : RobotState
{
    public IdleState(RobotStateMachineModule stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Robot.StatModule.SetStatus(RobotStatus.Idle);
        Robot.TaskModule.OnTaskStart.AddOnce(OnStartStart);
    }

    private void OnStartStart()
    {
        var des = Robot.TaskModule.PickupPoint.Position;
        StateMachine.ChangeState(new MoveState(StateMachine, des, () => StateMachine.ChangeState(new WaitingState(StateMachine))));
    }
}