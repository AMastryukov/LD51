using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutDisplay : MonoBehaviour
{
    [SerializeField] private LoadoutSlot activeWeaponSlot;
    [SerializeField] private LoadoutSlot activeTrapSlot;
    [SerializeField] private LoadoutSlot activeBonusSlot;

    [SerializeField] private LoadoutSlot nextWeaponSlot;
    [SerializeField] private LoadoutSlot nextTrapSlot;
    [SerializeField] private LoadoutSlot nextBonusSlot;

    private void Awake()
    {
        GameManager.OnNewWeapon += UpdateWeapons;
        GameManager.OnNewTrap += UpdateTraps;
        GameManager.OnNewBonus += UpdateBonuses;
    }

    private void OnDestroy()
    {
        GameManager.OnNewWeapon -= UpdateWeapons;
        GameManager.OnNewTrap -= UpdateTraps;
        GameManager.OnNewBonus -= UpdateBonuses;
    }

    private void UpdateWeapons(ItemData newWeapon, ItemData nextWeapon)
    {
        activeWeaponSlot.SetItem(newWeapon);
        nextWeaponSlot.SetItem(nextWeapon);
    }

    private void UpdateTraps(ItemData newTrap, ItemData nextTrap)
    {
        activeTrapSlot.SetItem(newTrap);
        nextTrapSlot.SetItem(nextTrap);
    }

    private void UpdateBonuses(Bonuses newBonus, Bonuses nextBonus)
    {
        activeBonusSlot.SetBonus(newBonus);
        nextBonusSlot.SetBonus(nextBonus);
    }
}
