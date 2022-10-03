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

    public static Action<ItemData, ItemData> OnNewWeapon;
    public static Action<ItemData, ItemData> OnNewTrap;
    public static Action<Buffs, Buffs> OnNewBuff;

    [Tooltip("The max count of each queue when populating; inclusive.")]
    [SerializeField] private int queueMax = 5;

    [Tooltip("When a queue reaches this amount, it will generate up to the count of queueMax")]
    [SerializeField] private int repopulateQueueAtCount = 2;

    [SerializeField] private ItemData[] traps = new ItemData[0];

    [SerializeField] private bool verboseLogging = false;
    [SerializeField] private bool superVerboseLogging = false;

    private Queue<ItemData> weaponQueue = new Queue<ItemData>();
    private Queue<ItemData> trapQueue = new Queue<ItemData>();
    private Queue<Buffs> buffQueue = new Queue<Buffs>();

    private ItemData lastQueuedWeapon = null;
    private ItemData lastQueuedTrap = null;
    private Buffs lastQueuedBuff = Buffs.PassivelyRegenerateHP;

    private GameStates gameState = GameStates.Menu;
    private IEnumerator gameTimer = null;
    private int buffsLength = 0;

    public GameStates GameState => gameState;

    //todo replace these
    private string[] tempWeapons = new string[3] { "Shotgun", "Revolver", "Katana" };

    private void OnValidate()
    {
        if (tempWeapons.Length <= 2)
        {
            Debug.LogError($"{nameof(tempWeapons)} must have at least 3 entries or its queue generation will never complete due to the 'no two of the same objects should be in the queue beside each other' rule.");
        }

        if (traps.Length <= 2)
        {
            Debug.LogError($"{nameof(traps)} must have at least 3 entries or its queue generation will never complete due to the 'no two of the same objects should be in the queue beside each other' rule.");
        }

        if (Enum.GetNames(typeof(Buffs)).Length <= 2)
        {
            Debug.LogError($"{nameof(Buffs)} must have at least 3 entries or its queue generation will never complete due to the 'no two of the same objects should be in the queue beside each other' rule.");
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

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        yield return new WaitForEndOfFrame();

        StartGame();
    }

    public void StartGame()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(StartGame), this);
        }

        buffsLength = Enum.GetNames(typeof(Buffs)).Length;

        SetGameState(GameStates.Play);
        StartGameTimer();

        GenerateWeaponQueue();
        GenerateTrapQueue();
        GenerateBuffsQueue();

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
        while (true)
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

            secondsRemaining = SECONDS_TO_COUNT_TO;
        }
    }

    private void ShiftQueues()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(ShiftQueues), this);
        }

        var newWeapon = weaponQueue.Dequeue();
        var nextWeapon = weaponQueue.Peek();
        OnNewWeapon?.Invoke(newWeapon, nextWeapon);

        var newTrap = trapQueue.Dequeue();
        var nextTrap = trapQueue.Peek();
        OnNewTrap?.Invoke(newTrap, nextTrap);

        Buffs newBuff = buffQueue.Dequeue();
        Buffs nextBuff = buffQueue.Peek();
        OnNewBuff?.Invoke(newBuff, nextBuff);

        if (weaponQueue.Count <= repopulateQueueAtCount)
        {
            GenerateWeaponQueue();
        }

        if (trapQueue.Count <= repopulateQueueAtCount)
        {
            GenerateTrapQueue();
        }

        if (buffQueue.Count <= repopulateQueueAtCount)
        {
            GenerateBuffsQueue();
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
            // TODO: Fetch a random ItemData that corresponds to a Weapon

            var newWeapon = new ItemData();
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
            int newTrapIndex = UnityEngine.Random.Range(0, traps.Length);
            ItemData newTrap = traps[newTrapIndex];

            while (newTrap == lastQueuedTrap)
            {
                newTrapIndex = UnityEngine.Random.Range(0, traps.Length);
                newTrap = traps[newTrapIndex];
            }

            trapQueue.Enqueue(newTrap);
            lastQueuedTrap = newTrap;
        }
    }

    private void GenerateBuffsQueue()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(GenerateBuffsQueue), this);
        }

        while (buffQueue.Count < queueMax)
        {
            int newBuffIndex = UnityEngine.Random.Range(0, buffsLength);
            Buffs newBuff = (Buffs)newBuffIndex;

            while (newBuff == lastQueuedBuff)
            {
                newBuffIndex = UnityEngine.Random.Range(0, buffsLength);
                newBuff = (Buffs)newBuffIndex;
            }

            buffQueue.Enqueue(newBuff);
            lastQueuedBuff = newBuff;
        }
    }
}
