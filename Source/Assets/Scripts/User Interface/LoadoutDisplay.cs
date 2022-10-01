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


    // TODO: Replace strings with ItemData when that has been implemented
    private void UpdateWeapons(string newWeapon, string nextWeapon)
    {
        // activeWeaponSlot.SetItem(newWeapon);
        // nextWeaponSlot.SetItem(nextWeapon);
    }

    // TODO: Replace strings with ItemData when that has been implemented
    private void UpdateTraps(string newTrap, string nextTrap)
    {
        // activeTrapSlot.SetItem(newTrap);
        // nextTrapSlot.SetItem(nextTrap);
    }

    private void UpdateBonuses(Bonuses newBonus, Bonuses nextBonus)
    {
        activeBonusSlot.SetBonus(newBonus);
        nextBonusSlot.SetBonus(nextBonus);
    }
}
