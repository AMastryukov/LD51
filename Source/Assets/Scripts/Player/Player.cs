using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerBonuses))]
public class Player : MonoBehaviour
{
    public static Action OnDie;
    public static Action<int> OnPlayerHealthChanged;
    public static Action<int> OnPlayerStaminaChanged;

    [Header("Health")]
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] [Tooltip("In Seconds")] private int regenerateFrequency = 10;
    [SerializeField] private int regenerateAmount = 1;

    [SerializeField] private int stamina = 100;
    [SerializeField] private int maxStamina = 100;

    [SerializeField] private bool verboseLogging = false;
    [SerializeField] private bool superVerboseLogging = false;

    private PlayerBonuses playerBonuses = null;
    private IEnumerator regenerateHealth = null;

    private Weapon activeWeapon = null;
    private ItemData activeTrap = null;
    private Item SelectedItem = null;

    public void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        TryGetComponent(out playerBonuses);

        GameManager.OnTenSecondsPassed += () =>
        {
            //DecrementHealth(5);
        };
    }

    #region Health

    public void DecrementHealth(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(DecrementHealth) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        SetHealth(health - amount);
    }

    public void IncrementHealth(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(IncrementHealth) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        SetHealth(health + amount);
    }

    public void SetHealth(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(SetHealth) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        health = Mathf.Clamp(amount, 0, maxHealth);

        if (amount == 0)
        {
            OnDie?.Invoke();
        }

        OnPlayerHealthChanged?.Invoke(health);

        if (health < maxHealth && regenerateHealth == null && playerBonuses.IsActive(Bonuses.PassivelyRegenerateHP))
        {
            regenerateHealth = RegenerateHealth(regenerateFrequency);
            StartCoroutine(regenerateHealth);
        }
        else if (health >= maxHealth && regenerateHealth != null)
        {
            StopRegeneratingHealth();
        }
    }

    private IEnumerator RegenerateHealth(int secondsToIncrement)
    {
        while (true)
        {
            while (secondsToIncrement > 0)
            {
                if (superVerboseLogging)
                {
                    Debug.Log(nameof(RegenerateHealth) + " ( " + nameof(secondsToIncrement) + ": " + secondsToIncrement + " )", this);
                }

                yield return new WaitForSeconds(1);
                secondsToIncrement--;
            }

            if (health >= maxHealth || !playerBonuses.IsActive(Bonuses.PassivelyRegenerateHP))
            {
                StopRegeneratingHealth();
            }
            else
            {
                IncrementHealth(regenerateAmount);

                if (health >= maxHealth)
                {
                    StopRegeneratingHealth();
                }

                secondsToIncrement = regenerateFrequency;

            }
        }
    }

    private void StopRegeneratingHealth()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(StopRegeneratingHealth), this);
        }

        StopCoroutine(regenerateHealth);
        regenerateHealth = null;
    }

    #endregion

    #region Stamina

    public void DecrementStamina(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(DecrementStamina) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        SetStamina(stamina - amount);
    }

    public void SetStamina(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(SetStamina) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        stamina = Mathf.Clamp(amount, 0, maxStamina);

        OnPlayerStaminaChanged?.Invoke(stamina);
    }

    #endregion
}
