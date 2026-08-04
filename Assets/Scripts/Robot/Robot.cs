using System.Threading.Tasks;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [field: SerializeField] public RobotStateMachineModule StateMachine { get; private set; }
    [field: SerializeField] public RobotMoveModule MoveModule { get; private set; }
    [field: SerializeField] public RobotStatModule StatModule { get; private set; }
    [field: SerializeField] public RobotFuelModule FuelModule { get; private set; }
    [field: SerializeField] public RobotSensorModule SensorModule { get; private set; }

    private bool _isInited;

    public async Task Init()
    {
        await StatModule.InitStat(this);
        MoveModule.Init(this);
        FuelModule.Init(this);
        StateMachine.Init(this);
        SensorModule.Init(this);

        _isInited = true;
    }

    void Update()
    {
        if (!_isInited) return;

        MoveModule.Tick();
        StateMachine.Tick();
        FuelModule.Tick();
    }
}