using UnityEngine;

public class Dashboard : MonoBehaviour
{
    [SerializeField] private Tabs tabs;

    void Start()
    {
        tabs.Init();
        tabs.SetTab(0, true);
    }
}