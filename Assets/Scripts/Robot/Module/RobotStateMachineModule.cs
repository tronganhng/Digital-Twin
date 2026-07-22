using Sirenix.OdinInspector;
using UnityEngine;

public class RobotStateMachineModule : RobotBaseModule
{
    [SerializeField, ReadOnly] private string stateName = "None";

    private IState _currentState;

    public Robot Robot => robot;
    public IState CurrentState => _currentState;

    public override void Init(Robot robot)
    {
        base.Init(robot);

        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(IState newState)
    {
        _currentState?.Exit();

        _currentState = newState;

        _currentState.Enter();

        stateName = newState.GetType().Name;
    }
}