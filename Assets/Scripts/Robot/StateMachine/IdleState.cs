using UnityEngine;

public class IdleState : RobotState
{
    protected override RobotStatus Status => RobotStatus.Idle;

    public IdleState(RobotStateMachineModule stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Robot.TaskModule.OnTaskStart.AddOnce(OnTaskStart);
    }

    private void OnTaskStart()
    {
        Robot.TaskModule.SetTaskStatus(TaskStatus.Running);
        var des = Robot.TaskModule.PickupPoint.Position;
        StateMachine.ChangeState(new MoveState(StateMachine, des, () => StateMachine.ChangeState(new WaitingState(StateMachine))));
    }
}