using System;
using System.Threading.Tasks;
using UnityEngine;

public class RobotSensorModule : RobotBaseModule
{
    void OnTriggerEnter(Collider other)
    {
        var mapPoint = other.GetComponentInParent<MapPoint>();
        if (mapPoint != null)
        {
            _ = TryAccessPoint(mapPoint);
        }
    }

    private async Task TryAccessPoint(MapPoint mapPoint)
    {
        var req = new ResourceAccessRequest
        {
            RobotId = robot.StatModule.StateDto.RobotId,
            PointName = mapPoint.PointName
        };
        var canAccess = await SimulationManager.Instance.WebSocket.SendRequestAsync<ResourceAccessRequest, bool>(SocketMessageType.ResourceAccess, req);
        if (!canAccess) ExtraLog.LogWithColor("Can not access this point", Color.yellow);
        else mapPoint.SetLock(true);
    }
}