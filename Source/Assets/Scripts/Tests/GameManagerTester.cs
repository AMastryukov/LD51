using System;
using TMPro;
using UnityEngine;

public class GameManagerTester : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameStateText = null;

    [SerializeField] private TextMeshProUGUI secondText = null;
    [SerializeField] private GameObject tenSecondsPassedText = null;

    [SerializeField] private GameObject startGameButton = null;
    [SerializeField] private GameObject endGameButton = null;

    [SerializeField] private bool verboseLogging = false;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

        GameManager.Instance.OnSecondPassed += OnSecond;
        GameManager.Instance.OnTenSecondsPassed += OnTenSecondsPassed;
        GameManager.Instance.OnTimerStopped += OnTimerStopped;
    }

    private void OnGameStateChanged(GameStates gameState)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnGameStateChanged) + " ( " + nameof(gameState) + ": " + gameState + " )", this);
        }

        gameStateText.text = "Game State: " + gameState.ToString();

        startGameButton.SetActive(gameState != GameStates.Play);
        endGameButton.SetActive(gameState == GameStates.Play);
    }

    private void OnSecond(int second)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnSecond) + " ( " + nameof(second) + ": " + second + " )", this);
        }

        secondText.text = second.ToString();

        if (second <= 9)
        {
            tenSecondsPassedText.SetActive(false);
        }
    }

    private void OnTenSecondsPassed()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnTenSecondsPassed), this);
        }

        tenSecondsPassedText.SetActive(true);
    }

    private void OnTimerStopped()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnTimerStopped), this);
        }

        secondText.text = string.Empty;
    }
}
