using System.Threading.Tasks;
using UnityEngine;

public class IdleState : RobotState
{
    public IdleState(RobotStateMachineModule stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Robot.MoveModule.Stop();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (Robot.FuelModule.NeedCharge)
        {
            var pole = SimulationManager.Instance.MapManager.GetFreeChargingPole(Robot);
            if (!pole) return;
            var des = pole.ChargePoint.position;
            StateMachine.ChangeState(new MoveState(StateMachine, des, () => StateMachine.ChangeState(new ChargingState(StateMachine, pole))));
        }
    }
}