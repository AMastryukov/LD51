using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    Menu,
    Play,
    End,
}

public class GameManager : MonoBehaviour
{
    private const int SECONDS_TO_COUNT_TO = 10;// change this if 10 seconds is too long for your attention span

    public static GameManager Instance { get; private set; }

    public static Action<GameStates> OnGameStateChanged;

    /// <summary>
    /// Sends seconds remaining out of 10
    /// </summary>
    public static Action<int> OnSecondPassed;
    public static Action OnTenSecondsPassed;
    public static Action OnTimerStarted;
    public static Action OnTimerStopped;

    public static Action<string, string> OnNewWeapon;
    public static Action<string, string> OnNewTrap;
    public static Action<Bonuses, Bonuses> OnNewBonus;

    [Tooltip("The max count of each queue when populating; inclusive.")]
    [SerializeField] private int queueMax = 5;

    [Tooltip("When a queue reaches this amount, it will generate up to the count of queueMax")]
    [SerializeField] private int repopulateQueueAtCount = 2;

    [SerializeField] private bool verboseLogging = false;
    [SerializeField] private bool superVerboseLogging = false;

    private Queue<string> weaponQueue = new Queue<string>();
    private Queue<string> trapQueue = new Queue<string>();
    private Queue<Bonuses> bonusQueue = new Queue<Bonuses>();

    private string lastQueuedWeapon = null;
    private string lastQueuedTrap = null;
    private Bonuses lastQueuedBonus = Bonuses.None;

    private GameStates gameState = GameStates.Menu;
    private IEnumerator gameTimer = null;
    private int bonusesLength = 0;

    public GameStates GameState => gameState;
    public Queue<string> WeaponQueue => weaponQueue;//todo make readonly
    public Queue<string> TrapQueue => trapQueue;//todo make readonly
    public Queue<Bonuses> BonusQueue => bonusQueue;//todo make readonly

    //todo replace these
    private string[] tempWeapons = new string[3] { "Shotgun", "Revolver", "Katana" };
    private string[] tempTraps = new string[3] { "Barbed Wire", "Turret ", "Mine" };

    private void OnValidate()
    {
        if (tempWeapons.Length <= 2)
        {
            Debug.LogError($"{nameof(tempWeapons)} must have at least 3 entries or its queue generation will never complete due to the 'no two of the same objects should be in the queue beside each other' rule.");
        }

        if (tempTraps.Length <= 2)
        {
            Debug.LogError($"{nameof(tempTraps)} must have at least 3 entries or its queue generation will never complete due to the 'no two of the same objects should be in the queue beside each other' rule.");
        }

        if (Enum.GetNames(typeof(Bonuses)).Length <= 2)
        {
            Debug.LogError($"{nameof(Bonuses)} must have at least 3 entries or its queue generation will never complete due to the 'no two of the same objects should be in the queue beside each other' rule.");
        }

        if (SECONDS_TO_COUNT_TO != 10)
        {
#pragma warning disable CS0162
            Debug.LogWarning($"Be sure to change {nameof(SECONDS_TO_COUNT_TO)} back to 10!");
#pragma warning restore CS0162
        }
    }

    private void Awake()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Awake), this);
        }

        Instance = this;
    }

    public void StartGame()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(StartGame), this);
        }

        bonusesLength = Enum.GetNames(typeof(Bonuses)).Length;

        SetGameState(GameStates.Play);
        StartGameTimer();

        GenerateWeaponQueue();
        GenerateTrapQueue();
        GenerateBonusQueue();

        ShiftQueues();
    }

    public void EndGame()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(EndGame), this);
        }

        SetGameState(GameStates.End);
        StopGameTimer();
    }

    public void StartGameTimer()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(StartGameTimer), this);
        }

        gameTimer = Count(SECONDS_TO_COUNT_TO);
        StartCoroutine(gameTimer);
        OnTimerStarted?.Invoke();
    }

    public void StopGameTimer()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(StopGameTimer), this);
        }

        StopCoroutine(gameTimer);
        gameTimer = null;
        OnTimerStopped?.Invoke();
    }

    public void SetGameState(GameStates gameState)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(SetGameState) + " ( " + nameof(gameState) + ": " + gameState + " )", this);
        }

        this.gameState = gameState;
        OnGameStateChanged?.Invoke(gameState);
    }

    private IEnumerator Count(int secondsRemaining)
    {
        while (secondsRemaining > 0)
        {
            if (superVerboseLogging)
            {
                Debug.Log(nameof(Count) + " ( " + nameof(secondsRemaining) + ": " + secondsRemaining + " )", this);
            }

            OnSecondPassed?.Invoke(secondsRemaining);
            yield return new WaitForSeconds(1);
            secondsRemaining--;
        }

        OnTenSecondsPassed?.Invoke();
        ShiftQueues();

        gameTimer = Count(SECONDS_TO_COUNT_TO);
        StartCoroutine(gameTimer);
    }

    private void ShiftQueues()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(ShiftQueues), this);
        }

        string newWeapon = weaponQueue.Dequeue();
        string nextWeapon = weaponQueue.Peek();
        OnNewWeapon?.Invoke(newWeapon, nextWeapon);

        string newTrap = trapQueue.Dequeue();
        string nextTrap = trapQueue.Peek();
        OnNewTrap?.Invoke(newTrap, nextTrap);

        Bonuses newBonus = bonusQueue.Dequeue();
        Bonuses nextBonus = bonusQueue.Peek();
        OnNewBonus?.Invoke(newBonus, nextBonus);

        if (weaponQueue.Count <= repopulateQueueAtCount)
        {
            GenerateWeaponQueue();
        }

        if (trapQueue.Count <= repopulateQueueAtCount)
        {
            GenerateTrapQueue();
        }

        if (bonusQueue.Count <= repopulateQueueAtCount)
        {
            GenerateBonusQueue();
        }
    }

    private void GenerateWeaponQueue()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(GenerateWeaponQueue), this);
        }

        while (weaponQueue.Count < queueMax)
        {
            int newWeaponIndex = UnityEngine.Random.Range(0, tempWeapons.Length);
            string newWeapon = tempWeapons[newWeaponIndex];

            while (newWeapon == lastQueuedWeapon)
            {
                newWeaponIndex = UnityEngine.Random.Range(0, tempWeapons.Length);
                newWeapon = tempWeapons[newWeaponIndex];
            }

            weaponQueue.Enqueue(newWeapon);
            lastQueuedWeapon = newWeapon;
        }
    }

    private void GenerateTrapQueue()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(GenerateTrapQueue), this);
        }

        while (trapQueue.Count < queueMax)
        {
            int newTrapIndex = UnityEngine.Random.Range(0, tempTraps.Length);
            string newTrap = tempTraps[newTrapIndex];

            while (newTrap == lastQueuedTrap)
            {
                newTrapIndex = UnityEngine.Random.Range(0, tempTraps.Length);
                newTrap = tempTraps[newTrapIndex];
            }

            trapQueue.Enqueue(newTrap);
            lastQueuedTrap = newTrap;
        }
    }

    private void GenerateBonusQueue()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(GenerateBonusQueue), this);
        }

        while (bonusQueue.Count < queueMax)
        {
            int newBonusIndex = UnityEngine.Random.Range(0, bonusesLength);
            Bonuses newBonus = (Bonuses)newBonusIndex;

            while (newBonus == lastQueuedBonus)
            {
                newBonusIndex = UnityEngine.Random.Range(0, bonusesLength);
                newBonus = (Bonuses)newBonusIndex;
            }

            bonusQueue.Enqueue(newBonus);
            lastQueuedBonus = newBonus;
        }
    }
}
