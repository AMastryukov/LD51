using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField]
    private Transform itemSocket;
    [SerializeField]
    private Transform itemParentSocket;
    private PlayerController playerController;


    [SerializeField] private Item itemPrefab;

    private Item activeItem;
    private GameObject itemHUD;

    [Header("WeaponBob")]
    [SerializeField] private float bobAmount = 0.05f;
    [SerializeField] private float bobFrequency = 10f;
    private float currentWeaponBobFactor;

    // Start is called before the first frame update
    void Start()
    {
        DebugUtility.HandleErrorIfNullGetComponent(itemSocket, this);
        DebugUtility.HandleErrorIfNullGetComponent(itemParentSocket, this);

        playerController = GetComponent<PlayerController>();
        DebugUtility.HandleErrorIfNullGetComponent(playerController, this);

        /************************************\
        |** Notify HUD about active object **|
        \************************************/

        Equip(itemPrefab);
    }

    public void Use()
    {
        activeItem.Use();
    }

    public void LateUpdate()
    {
        if (Time.deltaTime > 0f)
        {
            Vector3 playerCharacterVelocity =
                playerController.characterController.velocity / Time.deltaTime;

            // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
            float characterMovementFactor = 0f;
            if (playerController.IsGrounded)
            {
                characterMovementFactor =
                    Mathf.Clamp01(playerCharacterVelocity.magnitude /
                                  (playerController.MaxSpeed));
            }

            currentWeaponBobFactor =
                Mathf.Lerp(currentWeaponBobFactor, characterMovementFactor, bobFrequency * Time.deltaTime);

            // Calculate vertical and horizontal weapon bob values based on a sine function

            float hBobValue = Mathf.Sin(Time.time * bobFrequency) * bobAmount * currentWeaponBobFactor;
            float vBobValue = ((Mathf.Sin(Time.time * bobFrequency * 2f) * 0.5f) + 0.5f) * bobAmount *
                              currentWeaponBobFactor;

            // Apply weapon bob
            itemSocket.localPosition = new Vector3(hBobValue, vBobValue);
        }
    }

    public void Equip(Item item, int slot = 1)
    {
        activeItem = Instantiate(item, itemSocket);
    }
}
