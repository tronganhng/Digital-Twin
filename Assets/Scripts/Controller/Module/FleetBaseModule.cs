using UnityEngine;

public class FleetBaseModule : MonoBehaviour
{
    protected FleetManager fleet;
    
    public virtual void Init(FleetManager fleetManager)
    {
        fleet = fleetManager;
    }
}