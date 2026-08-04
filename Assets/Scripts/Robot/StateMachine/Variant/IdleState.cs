using System.Threading.Tasks;
using UnityEngine;

public class IdleState : RobotState
{
    protected override RobotStatus Status => RobotStatus.Idle;

    public IdleState(RobotStateMachineModule stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        Robot.MoveModule.Stop();
        CheckNodeFull();
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

    private void CheckNodeFull()
    {
        if (Robot.FuelModule.NeedCharge) return;
        var currentPoint = Robot.StatModule.CurrentMapPoint;
        if (currentPoint == null) return;

        var currentNode = currentPoint.Node;
        _ = SendReq();
        async Task SendReq()
        {
            var isFull = await SimulationManager.Instance.WebSocket.SendRequestAsync<string, bool>(SocketMessageType.CheckNodeFull, currentNode.NodeName);
            if (isFull)
            {
                // var waitingNode = SimulationManager.Instance.MapManager.WaitingNode;
                // StateMachine.ChangeState(new MoveToWaitingZoneState(StateMachine, waitingNode.Position, () =>
                //     StateMachine.ChangeState(new AcquireDockPointState(StateMachine, waitingNode, () =>
                //         StateMachine.ChangeState(new IdleState(StateMachine))))));
            }
        }
    }
}