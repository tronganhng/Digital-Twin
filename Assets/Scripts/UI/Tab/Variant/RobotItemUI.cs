using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RobotItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idTmp, statusTmp, batteryTmp;
    [SerializeField] private Button goChargeBtn;

    private Robot _robot;

    public void Init(Robot robot)
    {
        _robot = robot;

        _robot.StatModule.OnStatChanged.AddListener(UpdateUI);

        UpdateUI();
    }

    private void UpdateUI()
    {
        var state = _robot.StatModule.StateDto;
        idTmp.text = state.RobotId;
        statusTmp.text = state.Status.ToString();
        batteryTmp.text = $"{Math.Round(state.Battery, 0)}%";
        goChargeBtn.gameObject.SetActive(state.Battery <= 0);
    }

    public void MoveRobotToChargingPole()
    {
        // var des = pole.ChargePoint.position;
        // _robot.MoveModule.SetPosition(des);
        // _robot.SensorModule.ReleaseResource();
        // DOVirtual.DelayedCall(0.1f, () => _robot.StateMachine.ChangeState(new ChargingState(_robot.StateMachine)));
    }
}