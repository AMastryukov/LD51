using System;
using UnityEngine;

public class OnTriggerEnterHandler : MonoBehaviour
{
    public Action<Collider> OnTrigger;
    [SerializeField] private bool verboseLogging = false;

    private void OnTriggerEnter(Collider collider)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnTriggerEnter) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        OnTrigger?.Invoke(collider);
    }
}
