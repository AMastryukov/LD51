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

    public static void HandleEmptyLayerMask(LayerMask layerMask, MonoBehaviour mono, string expected = "<INSERT LAYER HERE>")
    {
#if UNITY_EDITOR
        if (layerMask.value == 0)
        {
            Debug.LogError("LayerMask missing on " + mono.name + ". Set this to the " + expected + " layer.");
        }
#endif
    }
}