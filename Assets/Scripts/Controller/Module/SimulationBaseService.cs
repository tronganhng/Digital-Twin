using UnityEngine;

public class SimulationBaseService : MonoBehaviour
{
    protected SimulationManager manager;
    
    public virtual void Init(SimulationManager manager)
    {
        this.manager = manager;
    }
}