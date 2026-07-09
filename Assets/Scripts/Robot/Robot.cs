using UnityEngine;

public class Robot : MonoBehaviour
{
    [SerializeField] public RobotStatus status;
    public float battery;
    [field: SerializeField] public RobotMoveModule MoveModule { get; private set; }

    public void Init(int robotId)
    {
        MoveModule.Init(this);
    }

    public void ApplyState(RobotStateDto state)
    {
        transform.position = state.position;
        transform.rotation = state.rotation;
    }
}
