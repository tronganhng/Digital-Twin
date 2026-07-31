using UnityEngine;

public class WaitingState : RobotState
{
    private const int WAIT_TIME = 3;

    private float _timer;
    
    protected override RobotStatus Status => RobotStatus.Moving;

    public WaitingState(RobotStateMachineModule stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        _timer = 0;
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= WAIT_TIME)
        {
            var desNode = Robot.TaskModule.DesNode;
            StateMachine.ChangeState(new MoveState(StateMachine, desNode.Position, () =>
                StateMachine.ChangeState(new AcquireDockPointState(StateMachine, desNode, () =>
                    Robot.TaskModule.SetTaskStatus(TaskStatus.Completed)))));
        }
    }
}