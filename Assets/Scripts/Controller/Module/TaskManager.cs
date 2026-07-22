using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

public class TaskManager : SimulationBaseService
{
    [SerializeField, ReadOnly] private List<DeliveryTask> tasks;

    [Title("Test")]
    [SerializeField] private MapManager map;
    [SerializeField, ValueDropdown(nameof(GetPointNames))] string pickupPoint;
    [SerializeField, ValueDropdown(nameof(GetPointNames))] string destinationPoint;
    [SerializeField] int priority;

    private IEnumerable<string> GetPointNames()
    {
        return map.GetPointNames();
    }

    [Button(ButtonSizes.Medium)]
    private void PickRandom()
    {
        var names = map.GetPointNames().ToList();

        if (names.Count < 2)
        {
            Debug.LogWarning("Need at least 2 map points.");
            return;
        }

        int pickupIndex = Random.Range(0, names.Count);

        int destinationIndex;
        do
        {
            destinationIndex = Random.Range(0, names.Count);
        }
        while (destinationIndex == pickupIndex);

        pickupPoint = names[pickupIndex];
        destinationPoint = names[destinationIndex];
    }

    [Button(ButtonSizes.Medium)]
    private async void AssignTask()
    {
        var task = new DeliveryTask
        {
            PickupLocation = pickupPoint,
            Destination = destinationPoint,
            Priority = priority,
        };

        var res = await SimulationManager.Instance.WebSocket.SendRequestAsync<DeliveryTask, DeliveryTask>(SocketMessageType.CreateTask, task);
        if (string.IsNullOrEmpty(res.TaskId))
        {
            ExtraLog.LogWithColor("Invalid Pickup & Destination", Color.yellow);
            return;
        }
        if (res != null && !tasks.Any(t => t.TaskId == res.TaskId)) tasks.Add(res);
    }

    public void UpdateTaskInfo(DeliveryTask newTaskInfo)
    {
        int index = tasks.FindIndex(t => t.TaskId == newTaskInfo.TaskId);

        if (index == -1)
        {
            tasks.Add(newTaskInfo);
            return;
        }

        tasks[index] = newTaskInfo;
    }
}