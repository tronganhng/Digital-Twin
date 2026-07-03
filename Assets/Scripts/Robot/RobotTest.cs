using Sirenix.OdinInspector;
using UnityEngine;

public class RobotTest : MonoBehaviour
{
    [SerializeField] private Robot robot;

    [Button]
    private void Move(Transform des)
    {
        robot.MoveModule.MoveTo(des);
    }
}