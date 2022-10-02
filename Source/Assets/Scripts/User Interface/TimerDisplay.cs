using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        GameManager.OnSecondPassed += UpdateTimer;
    }

    private void OnDestroy()
    {
        GameManager.OnSecondPassed -= UpdateTimer;
    }

    private void UpdateTimer(int secondsRemaining)
    {
        timerText.text = secondsRemaining.ToString();
    }
}
