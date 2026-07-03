using UnityEngine;

public class Robot : MonoBehaviour
{
    [field: SerializeField] public RobotMoveModule MoveModule { get; private set; }

    void Start()
    {
        MoveModule.Init(this);
    }
}
