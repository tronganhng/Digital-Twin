using UnityEngine;

public class RobotBaseModule : MonoBehaviour
{
    protected Robot robot;
    protected bool isInited;

    public virtual void Init(Robot robot)
    {
        this.robot = robot;
        isInited = true;
    }
}
