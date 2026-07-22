using UnityEngine;

public class Robot : MonoBehaviour
{
    [field: SerializeField] public RobotStateMachineModule StateMachine { get; private set; }
    [field: SerializeField] public RobotMoveModule MoveModule { get; private set; }
    [field: SerializeField] public RobotStatModule StatModule { get; private set; }
    [field: SerializeField] public RobotTaskModule TaskModule { get; private set; }

    public void Init()
    {
        StateMachine.Init(this);
        MoveModule.Init(this);
        StatModule.Init(this);
        TaskModule.Init(this);
    }
}