using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffsManager : MonoBehaviour
{
    private static PlayerBuffsManager instance = null;

    public static Action<Buffs, bool> OnBuffChange;

    private bool verboseLogging = false;

    private Dictionary<Buffs, bool> buffs = new Dictionary<Buffs, bool>();
    private Buffs activeBuff = Buffs.PassivelyRegenerateHP;

    public static PlayerBuffsManager Instance {
        get {
            if (!instance)
            {
                GameObject newGameObject = new GameObject(nameof(PlayerBuffsManager));
                instance = newGameObject.AddComponent<PlayerBuffsManager>();
            }

            return instance;
        }
    }

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

    public bool RoomOwned(Buffs buff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(RoomOwned) + " ( " + nameof(buff) + ": " + buff + " )", this);
        }

        return buffs[buff];
    }

    public bool IsBuffActive(Buffs buff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(IsBuffActive) + " ( " + nameof(buff) + ": " + buff + " )", this);
        }

        return activeBuff == buff && RoomOwned(buff);
    }

    private void OnNewBuff(BuffData activeBuff, BuffData nextBuff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnNewBuff) + " ( " + nameof(activeBuff.Buff) + ": " + activeBuff.Buff + " , " + nameof(nextBuff.Buff) + ": " + nextBuff.Buff + " )", this);
        }

        this.activeBuff = activeBuff.Buff;
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
