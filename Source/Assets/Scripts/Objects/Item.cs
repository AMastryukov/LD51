using System;
using UnityEngine;
using UnityEngine.Events;
public abstract class Item : MonoBehaviour
{
    public Action<string> StatusChanged;

    /// <summary>
    /// HUD can use this to get description/status/ammo count etc.
    /// </summary>
    public string Status {
        get { return Status; }
        set { StatusChanged.Invoke(value); Status = value; }
    }
    public abstract void Use();
}