using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class RobotSensorModule : RobotBaseModule
{
    [SerializeField, ReadOnly] private bool _disable;
    private MapNode _mapPoint;

    void OnTriggerEnter(Collider other)
    {
        if (_disable) return;

        if (_mapPoint) return;
        var mapPoint = other.GetComponentInParent<MapNode>();
        if (mapPoint)
        {
            _mapPoint = mapPoint;
            _ = TryAccessNode();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (_disable) return;

        if (!_mapPoint) return;

        _ = TryReleaseNode();
    }

    private async Task TryAccessNode()
    {
        var req = new ResourceAccessRequest
        {
            RobotId = robot.StatModule.StateDto.RobotId,
            PointName = _mapPoint.NodeName
        };
        bool canAccess = await SimulationManager.Instance.WebSocket.SendRequestAsync<ResourceAccessRequest, bool>(SocketMessageType.ResourceAccess, req);
        if (!canAccess)
        {
            robot.MoveModule.Pause();
        }
        else _mapPoint.SetLock(true);
    }

    private async Task TryReleaseNode()
    {
        var req = new ResourceAccessRequest
        {
            RobotId = robot.StatModule.StateDto.RobotId,
            PointName = _mapPoint.NodeName
        };

        string nextRobotId = await SimulationManager.Instance.WebSocket.SendRequestAsync<ResourceAccessRequest, string>(SocketMessageType.ResourceRelease, req);

        if (nextRobotId == string.Empty)
        {
            _mapPoint.SetLock(false);
        }
        else
        {
            var nextRobot = SimulationManager.Instance.RobotManager.GetRobotBy(nextRobotId);
            nextRobot.MoveModule.Resume();
        }

        _mapPoint = null;
    }

    public void Disable()
    {
        _disable = true;
    }

    public void Enable()
    {
        _disable = false;
    }

    public void ReleaseResource()
    {
        if (!_mapPoint) return;

        _ = TryReleaseNode();
    }
}