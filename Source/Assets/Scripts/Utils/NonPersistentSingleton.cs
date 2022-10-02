using UnityEngine;

public class NonPersistentSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    Debug.LogError("No Instance Found!");
                }
            }

            return _instance;
        }
    }
}
