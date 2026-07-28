using System;
using System.Threading.Tasks;
using UnityEngine;

public class RobotSensorModule : RobotBaseModule
{
    private MapPoint _mapPoint;

    void OnTriggerEnter(Collider other)
    {
        if (_mapPoint) return;
        var mapPoint = other.GetComponentInParent<MapPoint>();
        if (mapPoint)
        {
            _mapPoint = mapPoint;
            _ = TryAccessPoint();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!_mapPoint) return;

        _ = TryReleasePoint();
    }

    private async Task TryAccessPoint()
    {
        var req = new ResourceAccessRequest
        {
            RobotId = robot.StatModule.StateDto.RobotId,
            PointName = _mapPoint.PointName
        };
        var canAccess = await SimulationManager.Instance.WebSocket.SendRequestAsync<ResourceAccessRequest, bool>(SocketMessageType.ResourceAccess, req);
        if (!canAccess) ExtraLog.LogWithColor("Can not access this point", Color.yellow);
        else _mapPoint.SetLock(true);
    }

    private async Task TryReleasePoint()
    {
        var req = new ResourceAccessRequest
        {
            RobotId = robot.StatModule.StateDto.RobotId,
            PointName = _mapPoint.PointName
        };
        var isReleased = await SimulationManager.Instance.WebSocket.SendRequestAsync<ResourceAccessRequest, bool>(SocketMessageType.ResourceRelease, req);
        if (isReleased)
        {
            _mapPoint.SetLock(false);
            _mapPoint = null;
        }
    }
}