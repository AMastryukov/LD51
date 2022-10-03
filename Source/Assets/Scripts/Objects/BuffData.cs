using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffData", menuName = "BuffData", order = 1)]
public class BuffData : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public Buffs Buff;
}
