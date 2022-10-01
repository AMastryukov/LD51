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
    public static GameManager Instance { get; private set; }

    public Action<GameStates> OnGameStateChanged;

    /// <summary>
    /// Sends seconds remaining out of 10
    /// </summary>
    public Action<int> OnSecondPassed;
    public Action OnTenSecondsPassed;
    public Action OnTimerStarted;
    public Action OnTimerStopped;

    [SerializeField] private bool verboseLogging = false;
    [SerializeField] private bool superVerboseLogging = false;

    private GameStates gameState = GameStates.Menu;
    private IEnumerator gameTimer = null;

    public GameStates GameState => gameState;

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

        SetGameState(GameStates.Play);
        StartGameTimer();
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

        gameTimer = Count(10);
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

    public void Pause()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Pause), this);
        }

        StopCoroutine(gameTimer);
        gameTimer = null;
    }

    private void SetGameState(GameStates gameState)
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

        gameTimer = Count(10);
        StartCoroutine(gameTimer);
    }
}
