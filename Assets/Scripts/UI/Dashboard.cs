using UnityEngine;

public class Dashboard : MonoBehaviour
{
    [SerializeField] private Tabs tabs;
    [SerializeField] private RobotListUI robotListUI;

    public void Init()
    {
        tabs.Init();
        robotListUI.Init();
        tabs.SetTab(0, true);
    }
}