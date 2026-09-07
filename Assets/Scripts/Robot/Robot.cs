using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [field: SerializeField] public RobotStateMachineModule StateMachine { get; private set; }
    [field: SerializeField] public RobotMoveModule MoveModule { get; private set; }
    [field: SerializeField] public RobotStatModule StatModule { get; private set; }
    [field: SerializeField] public RobotFuelModule FuelModule { get; private set; }
    [field: SerializeField] public RobotSensorModule SensorModule { get; private set; }
    
    [SerializeField, ReadOnly] private ChargingPole chargingPole;

    private bool _isInited;

    public async Task Init()
    {
        await StatModule.InitStat(this);
        MoveModule.Init(this);
        FuelModule.Init(this);
        StateMachine.Init(this);
        SensorModule.Init(this);
        chargingPole = GetComponentInParent<RobotRoot>().ChargingPole;

        _isInited = true;
    }

    void Update()
    {
        if (!_isInited) return;

        MoveModule.Tick();
        StateMachine.Tick();
        FuelModule.Tick();
    }

    public void GoCharge()
    {
        StateMachine.ChangeState(new MoveState(StateMachine, chargingPole.ChargePoint.position, () =>
        {
            StateMachine.ChangeState(new ChargingState(StateMachine));
        }));
    }
}