using System;
using System.Collections;
using UnityEngine;

public class RegenerativeValue : MonoBehaviour
{
    public Action<int> OnChange;
    public Action<int> OnSecondPassed;

    private int currentValue = 100;
    private int maxValue = 100;
    [SerializeField] private int minValue = 0;
    [SerializeField] private bool autoRegenerate = false;
    [SerializeField] [Tooltip("In Seconds")] private int regenerateFrequency = 10;
    [SerializeField] private int regenerateAmount = 1;
    [SerializeField] private bool verboseLogging = false;
    [SerializeField] private bool superVerboseLogging = false;
    private IEnumerator regenerate = null;

    public int CurrentValue => currentValue;
    public int MaxValue => maxValue;
    public int MinValue => minValue;
    public bool CanStartRegeneration => currentValue < maxValue && regenerate == null;
    public bool RegenerationIsActive => regenerate != null;

    public void Decrement(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Decrement) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        SetCurrentValue(currentValue - amount);
    }

    public void Increment(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Increment) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        SetCurrentValue(currentValue + amount);
    }

    public void SetMaxValue(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(SetMaxValue) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        maxValue = amount;

        if (autoRegenerate && CanStartRegeneration)
        {
            StartRegeneration();
        }
    }

    public void SetCurrentValue(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(SetCurrentValue) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        int lastCurrentValue = currentValue;
        currentValue = Mathf.Clamp(amount, minValue, maxValue);
        if (lastCurrentValue != currentValue)
        {
            OnChange?.Invoke(currentValue);
        }

        if (autoRegenerate && CanStartRegeneration)
        {
            StartRegeneration();
        }
        else if (currentValue >= maxValue && RegenerationIsActive)
        {
            StopRegeneration();
        }
    }

    public void StartRegeneration()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(StartRegeneration), this);
        }

        regenerate = Regenerate(regenerateFrequency);
        StartCoroutine(regenerate);
    }

    public void StopRegeneration()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(StopRegeneration), this);
        }

        StopCoroutine(regenerate);
        regenerate = null;
    }

    private IEnumerator Regenerate(int secondsRemaining)
    {
        while (true)
        {
            while (secondsRemaining > 0)
            {
                if (superVerboseLogging)
                {
                    Debug.Log(nameof(Regenerate) + " ( " + nameof(secondsRemaining) + ": " + secondsRemaining + " )", this);
                }

                OnSecondPassed?.Invoke(secondsRemaining);
                yield return new WaitForSeconds(1);

                Increment(regenerateAmount);
                secondsRemaining--;
            }

            if (currentValue >= maxValue && regenerate != null)
            {
                StopRegeneration();
            }
            else
            {
                secondsRemaining = regenerateFrequency;
            }
        }
    }
}
