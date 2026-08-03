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
        Robot.TaskModule.OnTaskStart.AddOnce(OnTaskStart);
        CheckNodeFull();
    }

    public override void Exit()
    {
        base.Exit();
        Robot.TaskModule.OnTaskStart.RemoveOnce(OnTaskStart);
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

    private void OnTaskStart()
    {
        Robot.TaskModule.SetTaskStatus(TaskStatus.Running);
        var pickupNode = Robot.TaskModule.PickupNode;
        StateMachine.ChangeState(new MoveState(StateMachine, pickupNode.Position, () =>
            StateMachine.ChangeState(new AcquireDockPointState(StateMachine, pickupNode, () =>
                StateMachine.ChangeState(new WaitingState(StateMachine))))));
    }

    private void CheckNodeFull()
    {
        var currentPoint = Robot.StatModule.CurrentMapPoint;
        if (currentPoint == null) return;

        var currentNode = currentPoint.Node;
        _ = SendReq();
        async Task SendReq()
        {
            var isFull = await SimulationManager.Instance.WebSocket.SendRequestAsync<string, bool>(SocketMessageType.CheckNodeFull, currentNode.NodeName);
            if (isFull)
            {
                var pole = SimulationManager.Instance.MapManager.GetFreeChargingPole(Robot);
                if (!pole) return;
                var des = pole.ChargePoint.position;
                StateMachine.ChangeState(new MoveState(StateMachine, des, () => StateMachine.ChangeState(new ChargingState(StateMachine, pole))));
            }
            // if (SimulationManager.Instance.MapManager.TryGetNode(targetNodeDto.NodeName, out var targetNode))
            // {
            //     // Move to waiting zone
            // }
        }
    }
}