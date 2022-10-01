using UnityEngine;

public static class DebugUtility
{
    public static void HandleErrorIfNullGetComponent<TO, TS>(TO component, TS source)
    {
#if UNITY_EDITOR
        if (component == null)
        {
            Debug.LogError("Error: Component of type " + typeof(TO) + " is missing on " + typeof(TS) + ".");
        }
#endif
    }
}