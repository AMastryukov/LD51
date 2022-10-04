using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadoutDisplay : MonoBehaviour
{
    [SerializeField] private LoadoutSlot activeWeaponSlot;
    [SerializeField] private LoadoutSlot activeTrapSlot;
    [SerializeField] private LoadoutSlot activeBonusSlot;

    [SerializeField] private LoadoutSlot nextWeaponSlot;
    [SerializeField] private LoadoutSlot nextTrapSlot;
    [SerializeField] private LoadoutSlot nextBonusSlot;

    [SerializeField] private Transform swapIcon;

    private void Awake()
    {
        GameManager.OnNewWeapon += UpdateWeapons;
        GameManager.OnNewTrap += UpdateTraps;
        GameManager.OnNewBuff += UpdateBonuses;
        PlayerItemManager.OnItemSelected += UpdateHUD;
        PlayerItemManager.OnItemDepleted += ClearSlot;
    }

    private void OnDestroy()
    {
        GameManager.OnNewWeapon -= UpdateWeapons;
        GameManager.OnNewTrap -= UpdateTraps;
        GameManager.OnNewBuff -= UpdateBonuses;
        PlayerItemManager.OnItemSelected += UpdateHUD;
        PlayerItemManager.OnItemDepleted -= ClearSlot;
    }

    private void ClearSlot(int slot)
    {
        if (slot == 0)
        {
            activeWeaponSlot.DisableSlot();
        }
        else
        {
            activeWeaponSlot.DeselectSlot();
            activeTrapSlot.DisableSlot();
        }
    }

    private void UpdateHUD(int slot, Item item)
    {
        if (slot == 0)
        {
            activeWeaponSlot.SelectSlot();
            activeTrapSlot.DeselectSlot();
        }
        else
        {
            activeWeaponSlot.DeselectSlot();
            activeTrapSlot.SelectSlot();
        }
    }

    private void UpdateWeapons(ItemData newWeapon, ItemData nextWeapon)
    {
        activeWeaponSlot.SetItem(newWeapon);
        nextWeaponSlot.SetItem(nextWeapon);

        // Spin the swap icon
        swapIcon.DOKill(true);
        swapIcon.DORotate(swapIcon.rotation.eulerAngles + Vector3.back * 180f, 0.5f);
    }

    private void UpdateTraps(ItemData newTrap, ItemData nextTrap)
    {
        activeTrapSlot.SetItem(newTrap);
        nextTrapSlot.SetItem(nextTrap);
    }

    private void UpdateBonuses(BuffData newBonus, BuffData nextBonus)
    {
        activeBonusSlot.SetBonus(newBonus);
        nextBonusSlot.SetBonus(nextBonus);
    }
}
