using UnityEngine;

public abstract class RobotState : IState
{
    protected readonly RobotStateMachineModule StateMachine;
    protected readonly Robot Robot;


    protected RobotState(RobotStateMachineModule stateMachine)
    {
        StateMachine = stateMachine;
        Robot = stateMachine.Robot;
    }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }
}