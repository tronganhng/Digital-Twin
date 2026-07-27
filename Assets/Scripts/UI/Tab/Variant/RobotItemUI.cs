using System;
using TMPro;
using UnityEngine;

public class RobotItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idTmp, statusTmp, batteryTmp;

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
    }
}