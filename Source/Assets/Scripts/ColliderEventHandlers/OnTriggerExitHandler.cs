using System;
using UnityEngine;

public class OnTriggerExitHandler : MonoBehaviour
{
    public Action<Collider> OnTrigger;
    [SerializeField] private bool verboseLogging = false;

    private void OnTriggerExit(Collider collider)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnTriggerExit) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        OnTrigger?.Invoke(collider);
    }
}
