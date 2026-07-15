using UnityEngine;

public class Robot : MonoBehaviour
{
    [field: SerializeField] public RobotMoveModule MoveModule { get; private set; }
    [field: SerializeField] public RobotStatModule StatModule { get; private set; }

    public RobotTask CurrentTask { get; private set; }

    public void Init()
    {
        MoveModule.Init(this);
        StatModule.Init(this);
    }
}