using UnityEngine;

public class RobotRoot : MonoBehaviour
{
    [field: SerializeField] public Robot Robot { get; private set; }
    [field: SerializeField] public ChargingPole ChargingPole { get; private set; }
}