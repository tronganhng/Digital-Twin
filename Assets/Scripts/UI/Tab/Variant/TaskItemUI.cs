using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusTmp, priorityTmp, assignedRobot, locationTmp;
    [SerializeField] private Button cancelBtn;

    private DeliveryTask _task;

    public string TaskId => _task.TaskId;

    public void Init(DeliveryTask task)
    {
        if (_task != null) _task.OnDataChanged.RemoveListener(UpdateUI);
        _task = task;
        _task.OnDataChanged.AddListener(UpdateUI);
        UpdateUI();
    }

    private void UpdateUI()
    {
        cancelBtn.interactable = _task.Status != TaskStatus.Cancelled && _task.Status != TaskStatus.Completed;
        statusTmp.text = _task.Status.ToString();
        priorityTmp.text = $"Priority: {_task.Priority}";
        assignedRobot.text = _task.AssignedRobotId;
        locationTmp.text = $"{_task.PickupLocation} to {_task.Destination}";
    }

    public void CancelTask()
    {
        SimulationManager.Instance.TaskManager.CancelTask(_task);
    }
}