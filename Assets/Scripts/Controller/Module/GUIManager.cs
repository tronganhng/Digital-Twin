using UnityEngine;

public class GUIManager : SimulationBaseService
{
    [SerializeField] private Dashboard dashboard;
    [SerializeField] private LoggerUI loggerUI;

    public Dashboard Dashboard => dashboard;
    public LoggerUI LoggerUI => loggerUI;

    public override void Init(SimulationManager manager)
    {
        base.Init(manager);
        dashboard.Init();
    }
}