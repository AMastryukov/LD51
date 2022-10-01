using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum States
    {
        Idle,
        CountingToEnemyControl,
        CountingToPlayerControl,
    }

    public Action<States> OnStateChange;
    public Action<int> OnCountDown;
    public Action OnRoomLost;
    public Action OnRoomCaptured;

    [SerializeField] private Bonuses bonus = Bonuses.None;
    [SerializeField] private int enemySecondsToCaptureRoom = 10;
    [SerializeField] private int playerSecondsToCaptureRoom = 5;
    [SerializeField] private bool verboseLogging = false;

    private States state = States.Idle;
    private HashSet<Enemy> enemiesInRoom = new HashSet<Enemy>();
    private bool playerInRoom = false;
    private bool isControlledByPlayer = true;
    private IEnumerator countToRoomCapture = null;

    public States State => state;
    public bool IsControlledByPlayer => isControlledByPlayer;

    private void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        Debug.LogWarning($"{nameof(Room)} needs to hook up {nameof(OnEnemyDespawned)} to {nameof(EnemyPool)}'s on enemy despawned event!", this);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnTriggerEnter) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        if (collider.TryGetComponent(out Enemy enemy))
        {
            if (enemiesInRoom.Contains(enemy))
            {
                Debug.Log(nameof(OnTriggerEnter) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " ) | Somehow this enemy is entering but was already accounted for!", this);
            }
            else
            {
                enemiesInRoom.Add(enemy);
            }
        }
        else if (collider.TryGetComponent(out PlayerController playerController))
        {
            playerInRoom = true;
        }

        OnInhabitantsChanged();
    }

    private void OnTriggerExit(Collider collider)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnTriggerExit) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        if (collider.TryGetComponent(out Enemy enemy))
        {
            if (enemiesInRoom.Contains(enemy))
            {
                enemiesInRoom.Remove(enemy);
            }
            else
            {
                Debug.Log(nameof(OnTriggerExit) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " ) | Somehow this enemy is exiting but was not accounted for!", this);
            }
        }
        else if (collider.TryGetComponent(out PlayerController playerController))
        {
            playerInRoom = false;
        }

        OnInhabitantsChanged();
    }

    private void OnEnemyDespawned(GameObject enemyGameObject)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnEnemyDespawned) + " ( " + nameof(enemyGameObject) + ": " + enemyGameObject.name + " )", this);
        }

        if (enemyGameObject.TryGetComponent(out Enemy enemy))
        {
            if (enemiesInRoom.Contains(enemy))
            {
                enemiesInRoom.Remove(enemy);
                OnInhabitantsChanged();
            }
        }
        else
        {
            Debug.LogWarning($"{nameof(Room)} could not get the {nameof(Enemy)} component from the {nameof(OnEnemyDespawned)} event!", this);
        }
    }

    /// <summary>
    /// Triggered when a player or enemy enters or leaves the room
    /// </summary>
    private void OnInhabitantsChanged()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnInhabitantsChanged), this);
        }

        bool cancelCountToRoomCapture = enemiesInRoom.Count == 0 && !playerInRoom && countToRoomCapture != null;
        if (cancelCountToRoomCapture)
        {
            CancelCountToRoomCapture();
        }
        else
        {
            switch (state)
            {
                case States.Idle:

                    bool startEnemyCountToRoomCapture = isControlledByPlayer && enemiesInRoom.Count > 0 && !playerInRoom;
                    bool startPlayerCountToRoomCapture = !isControlledByPlayer && enemiesInRoom.Count == 0 && playerInRoom;

                    if (startEnemyCountToRoomCapture || startPlayerCountToRoomCapture)
                    {
                        int secondsToCaptureRoom = 10;

                        if (startEnemyCountToRoomCapture)
                        {
                            secondsToCaptureRoom = enemySecondsToCaptureRoom;
                            SetState(States.CountingToEnemyControl);
                        }
                        else if (startPlayerCountToRoomCapture)
                        {
                            secondsToCaptureRoom = playerSecondsToCaptureRoom;
                            SetState(States.CountingToPlayerControl);
                        }

                        countToRoomCapture = CountToRoomCapture(secondsToCaptureRoom);
                        StartCoroutine(countToRoomCapture);
                    }

                    break;
                case States.CountingToEnemyControl:

                    if (playerInRoom)
                    {
                        CancelCountToRoomCapture();
                    }

                    break;
                case States.CountingToPlayerControl:

                    if (enemiesInRoom.Count > 0)
                    {
                        CancelCountToRoomCapture();
                    }

                    break;
                default:
                    Debug.LogWarning($"{nameof(Room)}'s {nameof(OnInhabitantsChanged)} method has no support for a {nameof(States)} value of {state}!", this);
                    break;
            }
        }
    }

    private IEnumerator CountToRoomCapture(int secondsToCaptureRoom)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(CountToRoomCapture) + " ( " + nameof(secondsToCaptureRoom) + ": " + secondsToCaptureRoom + " )", this);
        }

        while (secondsToCaptureRoom > 0)
        {
            OnCountDown?.Invoke(secondsToCaptureRoom);
            yield return new WaitForSeconds(1);
            secondsToCaptureRoom--;
        }

        switch (state)
        {
            case States.CountingToEnemyControl:
                LoseControl();
                break;
            case States.CountingToPlayerControl:
                Capture();
                break;
            default:
                Debug.LogWarning($"{nameof(Room)}'s {nameof(CountToRoomCapture)} coroutine has no support for a {nameof(States)} value of {state}!", this);
                break;
        }

        SetState(States.Idle);
    }

    private void SetState(States state)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(SetState) + " ( " + nameof(state) + ": " + state + " )", this);
        }

        this.state = state;
        OnStateChange?.Invoke(state);
    }

    private void LoseControl()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(LoseControl), this);
        }

        isControlledByPlayer = false;
        OnRoomLost?.Invoke();
    }

    private void Capture()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Capture), this);
        }

        isControlledByPlayer = true;
        OnRoomCaptured?.Invoke();
    }

    private void CancelCountToRoomCapture()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(CancelCountToRoomCapture), this);
        }

        StopCoroutine(countToRoomCapture);
        countToRoomCapture = null;
        SetState(States.Idle);
    }
}
