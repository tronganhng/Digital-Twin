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

        idTmp.text = robot.StatModule.RobotId;
        statusTmp.text = robot.StatModule.Status.ToString();
        batteryTmp.text = $"{Math.Round(robot.StatModule.Battery, 1)}%";
    }
}