using UnityEngine;
using Sirenix.OdinInspector;

public class TaskManager : SimulationBaseService
{
    [Title("Test")]
    [SerializeField] string pickupPoint;
    [SerializeField] string destinationPoint;
    [SerializeField] int priority;

    [Button]
    private void AssignTask()
    {
        var task = new DeliveryTask
        {
            PickupLocation = pickupPoint,
            Destination = destinationPoint,
            Priority = priority,
        };

    }
}