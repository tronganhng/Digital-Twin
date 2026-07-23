using UnityEngine;
using UnityEngine.Events;

public class TabContent : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnable;
    [SerializeField] private UnityEvent onDisable;

    public void SetActive(bool isEnable)
    {
        gameObject.SetActive(isEnable);
        if (isEnable)
        {
            onEnable?.Invoke();
        }
        else
        {
            onDisable?.Invoke();
        }
    }
}