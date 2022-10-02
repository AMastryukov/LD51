using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    public static Action<Buffs, bool> OnBuffChange;

    [SerializeField] private bool verboseLogging = false;

    private Dictionary<Buffs, bool> buffs = new Dictionary<Buffs, bool>();
    private Buffs activeBuff = Buffs.PassivelyRegenerateHP;

    public bool this[Buffs key] => buffs[key];

    private void Awake()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Awake), this);
        }

        Array buffsValues = Enum.GetValues(typeof(Buffs));
        foreach (Buffs buffValue in buffsValues)
        {
            buffs.Add(buffValue, true);
        }

        Room.OnRoomLost += OnRoomLost;
        Room.OnRoomCaptured += OnRoomCaptured;

        GameManager.OnNewBuff += OnNewBuff;
    }

    public bool IsBuffActive(Buffs buff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(IsBuffActive) + " ( " + nameof(buff) + ": " + buff + " )", this);
        }

        return activeBuff == buff && this[buff];
    }

    private void OnNewBuff(Buffs activeBuff, Buffs nextBuff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnNewBuff) + " ( " + nameof(activeBuff) + ": " + activeBuff + " , " + nameof(nextBuff) + ": " + nextBuff + " )", this);
        }

        this.activeBuff = activeBuff;
    }

    private void OnRoomLost(Room room, Buffs buff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnRoomLost) + " ( " + nameof(room) + ": " + room.gameObject.name + " , " + nameof(buff) + ": " + buff + " )", this);
        }

        if (buffs[buff])
        {
            buffs[buff] = false;
            OnBuffChange?.Invoke(buff, buffs[buff]);
        }
    }

    private void OnRoomCaptured(Room room, Buffs buff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnRoomCaptured) + " ( " + nameof(room) + ": " + room.gameObject.name + " , " + nameof(buff) + ": " + buff + " )", this);
        }

        if (!buffs[buff])
        {
            buffs[buff] = true;
            OnBuffChange?.Invoke(buff, buffs[buff]);
        }
    }
}
