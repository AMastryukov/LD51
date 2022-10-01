using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBonuses : MonoBehaviour
{
    public static Action<Bonuses, bool> OnBonusChange;

    [SerializeField] private bool verboseLogging = false;

    private Dictionary<Bonuses, bool> bonuses = new Dictionary<Bonuses, bool>();

    public bool this[Bonuses key] => bonuses[key];

    private void Awake()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Awake), this);
        }

        Array bonusValues = Enum.GetValues(typeof(Bonuses));
        foreach (Bonuses bonusValue in bonusValues)
        {
            bonuses.Add(bonusValue, true);
        }
    }

    private void OnRoomLost(Room room, Bonuses bonus)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnRoomLost) + " ( " + nameof(room) + ": " + room.gameObject.name + " , " + nameof(bonus) + ": " + bonus + " )", this);
        }

        if (bonuses[bonus])
        {
            bonuses[bonus] = false;
            OnBonusChange?.Invoke(bonus, bonuses[bonus]);
        }
    }

    private void OnRoomCaptured(Room room, Bonuses bonus)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnRoomCaptured) + " ( " + nameof(room) + ": " + room.gameObject.name + " , " + nameof(bonus) + ": " + bonus + " )", this);
        }

        if (!bonuses[bonus])
        {
            bonuses[bonus] = true;
            OnBonusChange?.Invoke(bonus, bonuses[bonus]);
        }
    }
}
