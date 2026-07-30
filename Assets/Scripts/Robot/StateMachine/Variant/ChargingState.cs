public class ChargingState : RobotState
{
    protected override RobotStatus Status => RobotStatus.Charging;

    private ChargingPole _chargingPole;

    public ChargingState(RobotStateMachineModule stateMachine, ChargingPole chargingPole) : base(stateMachine)
    {
        _chargingPole = chargingPole;
    }

    public override void Enter()
    {
        base.Enter();
        Robot.FuelModule.StartCharging();
    }

    public override void Update()
    {
        base.Update();
        if (Robot.FuelModule.IsFull)
        {
            StateMachine.ChangeState(new IdleState(StateMachine));
        }
    }

    public override void Exit()
    {
        base.Exit();
        Robot.FuelModule.StopCharging();
        _chargingPole.Release();
    }
}