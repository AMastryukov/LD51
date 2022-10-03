using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemManagerState
{
    READY,
    LOWERED
}

public class PlayerItemManager : MonoBehaviour
{

    public static Action<int, Item> OnItemSelected;


    [Header("References")]
    [SerializeField]
    private Transform itemSocket;
    [SerializeField]
    private Transform itemParentSocket;
    private PlayerController playerController;


    [SerializeField] private ItemData ItemData;

    private Item activeItem;
    private GameObject itemHUD;

    [Header("WeaponBob")]
    [SerializeField] private float bobAmount = 0.05f;
    [SerializeField] private float bobFrequency = 10f;
    private float currentWeaponBobFactor;
    [SerializeField]
    private float maxLowerAmount = 1f;
    [SerializeField]
    private float lowerSpeed = 10f;
    private float lowerAmount = 0f;

    private Item[] currentItems = { null, null };

    private int selectedItem = 0;

    public const int ASSUMED_NUMBER_OF_ITEM_SLOTS = 2;

    public ItemManagerState State { get; private set; } = ItemManagerState.READY;

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

        if (ItemData != null) EquipNextItems(ItemData, ItemData);
    }

    private void Update()
    {
        switch (State)
        {
            case ItemManagerState.READY:
                // Don't need to do anything
                break;
            case ItemManagerState.LOWERED:
                lowerAmount = Mathf.Max(lowerAmount - lowerSpeed * Time.deltaTime, 0);
                if (lowerAmount == 0)
                {
                    State = ItemManagerState.READY;
                }
                break;
            default:
                break;
        }
    }

    private void Awake()
    {
        GameManager.OnNextItems += EquipNextItems;
    }

    private void OnDestroy()
    {
        GameManager.OnNextItems -= EquipNextItems;
    }

    void EquipNextItems(ItemData nextWeapon, ItemData nextTrap)
    {
        Destroy(currentItems[0]?.gameObject);
        Destroy(currentItems[1]?.gameObject);

        currentItems[0] = Instantiate(nextWeapon.Item, itemSocket);
        currentItems[1] = Instantiate(nextTrap.Item, itemSocket);

        Equip(selectedItem, true);
    }

    void LowerItem()
    {
        State = ItemManagerState.LOWERED;
        lowerAmount = maxLowerAmount;

    }

    public void Use(bool held)
    {
        if (State == ItemManagerState.READY && activeItem != null)
        {
            if (activeItem.Use(held))
            {
                Destroy(activeItem.gameObject);
                currentItems[selectedItem] = null;
                Equip((selectedItem + 1) % ASSUMED_NUMBER_OF_ITEM_SLOTS);

            }
        }

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
            itemSocket.localPosition = new Vector3(hBobValue, vBobValue - lowerAmount);
        }
    }

    public void Equip(int slot, bool force = false)
    {
        if ((slot != selectedItem || force) && currentItems[slot] != null)
        {
            currentItems[slot]?.gameObject.SetActive(true);
            currentItems[(slot + 1) % ASSUMED_NUMBER_OF_ITEM_SLOTS]?.gameObject.SetActive(false);
            LowerItem();
            activeItem = currentItems[slot];
            selectedItem = slot;
            OnItemSelected?.Invoke(slot, activeItem);
        }
    }
}
