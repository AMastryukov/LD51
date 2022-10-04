using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSlot : MonoBehaviour
{
    [SerializeField] private Image image;

    private Outline _outline;

    // The slot can either have an ItemData or a Bonus
    private ItemData _itemData;
    private Buffs _bonus;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    public void SetItem(ItemData data)
    {
        _itemData = data;
        image.sprite = _itemData.Sprite;

        EnableSlot();
    }

    public void SetBonus(BuffData bonus)
    {
        _bonus = bonus.Buff;
        image.sprite = bonus.Sprite;

        EnableSlot();
    }

    public void SelectSlot()
    {
        if (_outline != null)
            _outline.enabled = true;
    }

    public void DeselectSlot()
    {
        if (_outline != null)
            _outline.enabled = false;
    }

    public void DisableSlot()
    {
        image.color = Color.black;

        if (_outline != null)
            _outline.enabled = false;
    }

    private void EnableSlot()
    {
        image.color = Color.white;
    }
}
