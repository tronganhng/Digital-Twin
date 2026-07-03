using UnityEngine;

public class RobotBaseModule : MonoBehaviour
{
    protected Robot robot;

    public virtual void Init(Robot robot)
    {
        this.robot = robot;
    }
}
