using System.Threading.Tasks;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [field: SerializeField] public RobotStateMachineModule StateMachine { get; private set; }
    [field: SerializeField] public RobotMoveModule MoveModule { get; private set; }
    [field: SerializeField] public RobotStatModule StatModule { get; private set; }
    [field: SerializeField] public RobotTaskModule TaskModule { get; private set; }
    [field: SerializeField] public RobotFuelModule FuelModule { get; private set; }

    public async Task Init()
    {
        await StatModule.InitStat(this);
        MoveModule.Init(this);
        TaskModule.Init(this);
        FuelModule.Init(this);
        StateMachine.Init(this);
    }
}