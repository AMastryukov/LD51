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

        room.OnStateChange += OnStateChange;
        room.OnCountDown += OnCountDown;
        room.OnRoomLost += OnRoomLost;
        room.OnRoomCaptured += OnRoomCaptured;

        OnStateChange(room.State);

        if (room.IsControlledByPlayer)
        {
            OnRoomCaptured();
        }
        else
        {
            OnRoomLost();
        }
    }

    private void OnStateChange(Room.States state)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnStateChange) + " ( " + nameof(state) + ": " + state + " )", this);
        }

        roomStateText.text = state.ToString();

        if (state == Room.States.Idle)
        {
            countDownText.text = string.Empty;
        }
    }

    private void OnCountDown(int secondsLeft)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnCountDown) + " ( " + nameof(secondsLeft) + ": " + secondsLeft + " )", this);
        }

        countDownText.text = secondsLeft.ToString();
    }

    private void OnRoomLost()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnRoomLost), this);
        }

        roomControlText.text = "Under Enemy Control!";
    }

    private void OnRoomCaptured()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnRoomCaptured), this);
        }

        roomControlText.text = "Tis yours!";
    }
}
