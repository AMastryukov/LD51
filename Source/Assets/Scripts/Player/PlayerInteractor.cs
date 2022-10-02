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

        DebugUtility.HandleEmptyLayerMask(interactableLayerMask, this, "Interactable");
    }

    private void Update()
    {
        if (inputHandler.GetInteractInput())
        {
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
