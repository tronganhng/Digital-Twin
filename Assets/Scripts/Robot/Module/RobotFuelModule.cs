using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RobotFuelModule : RobotBaseModule
{
    [Header("Config")]
    [SerializeField] private float drainPerSecond = 0.5f;
    [SerializeField] private float chargePerSecond = 5f;

    [Header("UI")]
    [SerializeField] private Image batteryFill;
    [SerializeField] private TextMeshProUGUI batteryTmp;

    private bool _isCharging;

    private void Update()
    {
        if (!isInited) return;
        UpdateBattery();
        RefreshUI();
    }

    private void UpdateBattery()
    {
        double battery = robot.StatModule.Battery;

        if (_isCharging)
            battery += chargePerSecond * Time.deltaTime;
        else
            battery -= drainPerSecond * Time.deltaTime;

        battery = Mathf.Clamp((float)battery, 0f, 100f);

        robot.StatModule.Battery = battery;

        if (battery <= 0)
        {
            robot.StateMachine.ChangeState(new OfflineState(robot.StateMachine));
        }
    }

    private void RefreshUI()
    {
        float percent = (float)robot.StatModule.Battery / 100f;

        batteryFill.fillAmount = percent;
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
}