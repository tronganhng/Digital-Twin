using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RobotFuelModule : RobotBaseModule
{
    [Header("Config")]
    [SerializeField] private float drainPerSecond = 0.5f;
    [SerializeField] private float chargePerSecond = 5f;
    [SerializeField] private float needChargeThreshold = 20;
    [SerializeField] private float fullChargeThreshold = 100;

    [Header("UI")]
    [SerializeField] private Image batteryFill;
    [SerializeField] private TextMeshProUGUI batteryTmp;

    private bool _isCharging;

    public bool NeedCharge => robot.StatModule.Battery <= needChargeThreshold;
    public bool IsFull => robot.StatModule.Battery >= fullChargeThreshold;

    public override void Tick()
    {
        base.Tick();

        UpdateBattery();
        RefreshUI();
    }

    private void UpdateBattery()
    {
        double battery = robot.StatModule.Battery;
        
        if (battery <= 0) return;

        if (_isCharging)
            battery += chargePerSecond * Time.deltaTime;
        else
            battery -= drainPerSecond * Time.deltaTime;

        battery = Mathf.Clamp((float)battery, 0f, 100f);

        robot.StatModule.Battery = battery;

        ThresholdHandler(battery);
    }

    private void RefreshUI()
    {
        float percent = (float)robot.StatModule.Battery / 100f;

        batteryFill.fillAmount = percent;
        batteryFill.color = percent > needChargeThreshold / 100 ? Color.green : Color.red;
        batteryTmp.text = $"{robot.StatModule.Battery:0}%";
    }

    public void StartCharging()
    {
        _isCharging = true;
    }

    public void StopCharging()
    {
        _isCharging = false;
    }

    private void ThresholdHandler(double battery)
    {
        if (battery <= 0)
        {
            robot.StateMachine.ChangeState(new OfflineState(robot.StateMachine));
        }
    }

    [Button(ButtonSizes.Medium)]
    private void FillFuel()
    {
        robot.StatModule.Battery = 100;
        robot.StateMachine.ChangeState(new IdleState(robot.StateMachine));
    }
}