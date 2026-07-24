public class OfflineState : RobotState
{
    protected override RobotStatus Status => RobotStatus.Offline;

    public OfflineState(RobotStateMachineModule stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Robot.StatModule.Disable();
        base.Enter();
        Robot.MoveModule.Stop();
    }

    public override void Exit()
    {
        base.Exit();
        Robot.StatModule.Enable();
    }
}