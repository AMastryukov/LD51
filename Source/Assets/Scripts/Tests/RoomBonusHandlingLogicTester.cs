using TMPro;
using UnityEngine;

public class RoomBonusHandlingLogicTester : MonoBehaviour
{
    [SerializeField] private Room room = null;

    [SerializeField] private TextMeshProUGUI roomStateText = null;
    [SerializeField] private TextMeshProUGUI countDownText = null;
    [SerializeField] private TextMeshProUGUI roomControlText = null;

    [SerializeField] private bool verboseLogging = false;

    private void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        Room.OnStateChange += OnStateChange;
        Room.OnCountDown += OnCountDown;
        Room.OnRoomLost += OnRoomLost;
        Room.OnRoomCaptured += OnRoomCaptured;

        OnStateChange(room, room.State);

        if (room.IsControlledByPlayer)
        {
            OnRoomCaptured(room, room.Buff);
        }
        else
        {
            OnRoomLost(room, room.Buff);
        }
    }

    private void OnStateChange(Room room, Room.States state)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnStateChange) + " ( " + nameof(room) + ": " + room.gameObject.name + " , " + nameof(state) + ": " + state + " )", this);
        }

        roomStateText.text = state.ToString();

        if (state == Room.States.Idle)
        {
            countDownText.text = string.Empty;
        }
    }

    private void OnCountDown(Room room, int secondsLeft)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnCountDown) + " ( " + nameof(room) + ": " + room.gameObject.name + " , " + nameof(secondsLeft) + ": " + secondsLeft + " )", this);
        }

        countDownText.text = secondsLeft.ToString();
    }

    private void OnRoomLost(Room room, Buffs bonus)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnRoomLost) + " ( " + nameof(room) + ": " + room.gameObject.name + " , " + nameof(bonus) + ": " + bonus + " )", this);
        }

        roomControlText.text = "Under Enemy Control!";
    }

    private void OnRoomCaptured(Room room, Buffs bonus)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnRoomCaptured) + " ( " + nameof(room) + ": " + room.gameObject.name + " , " + nameof(bonus) + ": " + bonus + " )", this);
        }

        roomControlText.text = "Tis yours!";
    }
}
