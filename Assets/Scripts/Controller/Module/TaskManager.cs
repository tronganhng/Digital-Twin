using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class TaskManager : SimulationBaseService
{
    [SerializeField, ReadOnly] private List<DeliveryTask> tasks;

    [Title("Test")]
    [SerializeField] string pickupPoint;
    [SerializeField] string destinationPoint;
    [SerializeField] int priority;

    [Button]
    private async void AssignTask()
    {
        var task = new DeliveryTask
        {
            PickupLocation = pickupPoint,
            Destination = destinationPoint,
            Priority = priority,
        };

        var res = await SimulationManager.Instance.WebSocket.SendRequestAsync<DeliveryTask, DeliveryTask>(SocketMessageType.CreateTask, task);
        if (res != null) tasks.Add(res);
    }

    public void UpdateTaskInfo(DeliveryTask newTaskInfo)
    {
        var task = tasks.Find(t => t.TaskId == newTaskInfo.TaskId);
        if (task == null)
            return;

        task.CopyFrom(newTaskInfo);
    }
}