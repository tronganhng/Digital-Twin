using Sirenix.OdinInspector;
using UnityEngine;

public class SimulationManager : MonoSingleton<SimulationManager>
{
    [field: SerializeField] public RobotManager RobotManager { get; private set; }
    [field: SerializeField] public TaskManager TaskManager { get; private set; }
    [field: SerializeField] public MapManager MapManager { get; private set; }
    [field: SerializeField] public MessageRouter Router { get; private set; }
    [field: SerializeField] public WebSocketClient WebSocket { get; private set; }
    [field: SerializeField] public GUIManager GUI { get; private set; }

    private async void Start()
    {
        await WebSocket.InitWebSocket(this);
        Router.Init(this);
        MapManager.Init(this);
        await RobotManager.InitRobot(this);
        TaskManager.Init(this);
        GUI.Init(this);
        ExtraLog.LogWithColor("All service init success!", Color.green);
    }
}