using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObjects/", order = 1)]
public class ItemData : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public Item Item;
}
