using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public bool IsLookingAtInteractable { get { return lookingAtInteractable != null; } }

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private Transform cameraTransform;

    private IInteractable lookingAtInteractable = null;

    private PlayerInputHandler inputHandler;

    private void Start()
    {
        // Component that are on this gameobject
        inputHandler = GetComponent<PlayerInputHandler>();
        DebugUtility.HandleErrorIfNullGetComponent(inputHandler, this);

        if (interactableLayerMask.value == 0)
        {
            Debug.LogWarning("LayerMask missing on " + gameObject.name + ". Set this to the Interactable layer");
        }
    }

    private void Update()
    {
        if (inputHandler.GetInteractInput())
        {
            Debug.Log("Pressed e");
            lookingAtInteractable?.Interact();

            Debug.DrawRay(cameraTransform.position, cameraTransform.forward, Color.blue, 2f);

        }
    }

    private void FixedUpdate()
    {
        CastInteractionRay();
    }

    private void CastInteractionRay()
    {
        RaycastHit hit;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionDistance, interactableLayerMask))
        {
            lookingAtInteractable = hit.collider.GetComponent<IInteractable>();
            return;
        }



        lookingAtInteractable = null;
    }
}
