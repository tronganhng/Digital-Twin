using System;
using TMPro;
using UnityEngine;

public class RobotItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idTmp, statusTmp, batteryTmp;

    private RobotStateDto _robot;

    public void Init(RobotStateDto robot)
    {
        _robot = robot;

        _robot.OnStatChanged.AddListener(UpdateUI);

        UpdateUI();
    }

    private void UpdateUI()
    {
        idTmp.text = _robot.RobotId;
        statusTmp.text = _robot.Status.ToString();
        batteryTmp.text = $"{Math.Round(_robot.Battery, 0)}%";
    }
}