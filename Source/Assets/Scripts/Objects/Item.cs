using System;
using UnityEngine;
using UnityEngine.Events;
public abstract class Item : MonoBehaviour
{
    public static Action<string> OnStatusChanged;

    /// <summary>
    /// HUD can use this to get description/status/ammo count etc.
    /// </summary>
    public string Status {
        get { return Status; }
        set { OnStatusChanged?.Invoke(value); Status = value; }
    }
    public abstract void Use();
}