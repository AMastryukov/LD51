using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour
{

    [SerializeField]
    private Transform itemSocket;

    [SerializeField] private Item itemPrefab;

    private Item activeItem;
    private GameObject ItemHUD;

    // Start is called before the first frame update
    void Start()
    {
        DebugUtility.HandleErrorIfNullGetComponent(itemSocket, this);

        /************************************\
        |** Notify HUD about active object **|
        \************************************/


        Equip(itemPrefab);
    }

    public void Use()
    {
        activeItem.Use();
    }

    public void Equip(Item item, int slot = 1)
    {
        activeItem = Instantiate(item, itemSocket);
    }
}
