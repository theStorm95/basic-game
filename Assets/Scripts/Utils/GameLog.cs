using UnityEngine;

public static class GameLog
{
    public static void Error(string tag, string msg) =>
        Debug.LogError($"[{tag}] {msg}");

    public static void Warn(string tag, string msg) =>
        Debug.LogWarning($"[{tag}] {msg}");

    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Info(string tag, string msg) =>
        Debug.Log($"[{tag}] {msg}");
}
