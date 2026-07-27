using UnityEngine;

public class GUIManager : SimulationBaseService
{
    [SerializeField] private Dashboard dashboard;

    public Dashboard Dashboard => dashboard;

    public override void Init(SimulationManager manager)
    {
        base.Init(manager);
        dashboard.Init();
    }
}