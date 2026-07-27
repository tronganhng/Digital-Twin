using TMPro;
using UnityEngine;

public class TaskItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusTmp, assignedRobot;

    private DeliveryTask _task;

    public void Init(DeliveryTask task)
    {
        _task = task;

        _task.OnDataChanged.AddListener(UpdateUI);

        UpdateUI();
    }

    private void UpdateUI()
    {
        statusTmp.text = _task.Status.ToString();
        assignedRobot.text = _task.AssignedRobotId;
    }
}