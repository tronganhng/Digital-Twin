using System.Collections.Generic;
using UnityEngine;

public class Tabs : MonoBehaviour
{
    [SerializeField] private List<TabButton> buttons;

    public List<TabButton> Buttons => buttons;
    
    public void Init()
    {
        for (var i = 0; i < buttons.Count; i++)
        {
            var tabIndex = i;
            buttons[tabIndex].Init();
            buttons[tabIndex].onClick = () => SetTab(tabIndex, false);
        }
    }

    public void SetTab(int index, bool instant = true)
    {
        for (var i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetSelected(i == index, instant);
        }
    }
}