using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSlot : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image lockedImage;

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
    }

    public void SetBonus(BuffData bonus)
    {
        _bonus = bonus.Buff;
        image.sprite = bonus.Sprite;
    }

    public void SelectSlot()
    {
        _outline.enabled = true;
    }

    public void DeselectSlot()
    {
        _outline.enabled = false;
    }

    public void ClearSlot()
    {
        image.sprite = null;
        _outline.enabled = false;
    }
}
