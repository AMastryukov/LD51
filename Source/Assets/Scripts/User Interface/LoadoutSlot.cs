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
    }

    public void SetBonus(Buffs bonus)
    {
        _bonus = bonus;
        // TODO: find sprite associated with the bonus
    }

    public void SelectSlot()
    {
        _outline.enabled = true;
    }

    public void DeselectSlot()
    {
        _outline.enabled = false;
    }
}
