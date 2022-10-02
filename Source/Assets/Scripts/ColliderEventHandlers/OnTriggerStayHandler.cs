using System;
using UnityEngine;

public class OnTriggerStayHandler : MonoBehaviour
{
    public Action<Collider> OnTrigger;
    [SerializeField] private bool verboseLogging = false;

    private void OnTriggerStay(Collider collider)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnTriggerStay) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        OnTrigger?.Invoke(collider);
    }
}
