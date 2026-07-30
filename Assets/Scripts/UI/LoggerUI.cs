using TMPro;
using UnityEngine;

public class LoggerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageTmp;

    public void LogWithColor(object message, Color color)
    {
        if (messageTmp == null)
            return;

        messageTmp.text = message?.ToString() ?? string.Empty;
        messageTmp.color = color;
    }
}