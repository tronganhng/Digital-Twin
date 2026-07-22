using UnityEngine;

public class WaitingState : RobotState
{
    private const int WAIT_TIME = 3;

    private float _timer;

    public WaitingState(RobotStateMachineModule stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Robot.StatModule.SetStatus(RobotStatus.Moving);
        _timer = 0;
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= WAIT_TIME)
        {
            var des = Robot.TaskModule.DesPoint.Position;
            StateMachine.ChangeState(new MoveState(StateMachine, des, () => StateMachine.ChangeState(new IdleState(StateMachine))));
        }
    }
}