using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSlot : MonoBehaviour
{
    [SerializeField] private Image image;

    private Outline _outline;
    // private ItemData _itemData;
    private Bonuses _bonus;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    /** Uncomment this when ItemData has been implemented
    public void SetItem(string data)
    {
        _itemData = data;
        itemImage.sprite = _itemData.Sprite;
    }
    **/

    public void SetBonus(Bonuses bonus)
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
