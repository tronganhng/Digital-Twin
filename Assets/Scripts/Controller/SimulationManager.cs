using Sirenix.OdinInspector;
using UnityEngine;

public class SimulationManager : MonoSingleton<SimulationManager>
{
    [field: SerializeField] public RobotManager RobotManager { get; private set; }
    [field: SerializeField] public MapManager MapManager { get; private set; }
    [field: SerializeField] public WebSocketClient WebSocket { get; private set; }

    private async void Start()
    {
        await WebSocket.InitWebSocket(this);
        MapManager.Init(this);
        RobotManager.Init(this);
        Debug.Log("All service init success!");
    }

    [Title("Test")]
    [SerializeField] string pickupPoint;
    [SerializeField] string destinationPoint;

    [Button]
    private void AssignTask()
    {

    }
}