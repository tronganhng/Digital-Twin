using UnityEngine;

public static class ExtraLog
{
    public static void LogWithColor(object message, Color color)
    {
        string hex = ColorUtility.ToHtmlStringRGB(color);
        Debug.Log($"<color=#{hex}>{message}</color>");
        if (Application.isPlaying) SimulationManager.Instance.GUI.LoggerUI.LogWithColor(message, color);
    }

    public static void Log(object message)
    {
        Debug.Log($"{message}");
    }
}