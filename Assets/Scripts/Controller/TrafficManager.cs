using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class TrafficManager : FleetBaseModule
{
    [SerializeField, ReadOnly] private SerializedDictionary<string, int> _reservations = new();

    /// <summary>
    /// Robot yêu cầu sử dụng một resource.
    /// </summary>
    public bool TryReserve(string resourceId, int robotId)
    {
        if (!_reservations.TryGetValue(resourceId, out int owner))
        {
            _reservations.Add(resourceId, robotId);
            return true;
        }

        // Robot đã giữ từ trước
        if (owner == robotId)
            return true;

        return false;
    }

    /// <summary>
    /// Giải phóng resource.
    /// </summary>
    public void Release(string resourceId, int robotId)
    {
        if (!_reservations.TryGetValue(resourceId, out int owner))
            return;

        if (owner != robotId)
            return;

        _reservations.Remove(resourceId);
    }

    /// <summary>
    /// Resource đang được sử dụng?
    /// </summary>
    public bool IsOccupied(string resourceId)
    {
        return _reservations.ContainsKey(resourceId);
    }

    /// <summary>
    /// Robot nào đang giữ resource.
    /// </summary>
    public int? GetOwner(string resourceId)
    {
        if (_reservations.TryGetValue(resourceId, out int owner))
            return owner;

        return null;
    }

    /// <summary>
    /// Xóa toàn bộ reservation.
    /// </summary>
    public void Clear()
    {
        _reservations.Clear();
    }
}