public class OfflineState : RobotState
{
    protected override RobotStatus Status => RobotStatus.Offline;

    public OfflineState(RobotStateMachineModule stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }
}