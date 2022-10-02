using System;
using UnityEngine;

[RequireComponent(typeof(PlayerBuffs))]
public class Player : MonoBehaviour
{
    public static Action OnDie;
    public static Action<int> OnPlayerHealthChanged;
    public static Action<int, int> OnPlayerStaminaChanged;

    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private RegenerativeValue healthRegenerativeValue = null;

    [SerializeField] private int stamina = 100;
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private int maxStaminaWithBuff = 200;
    [SerializeField] private RegenerativeValue staminaRegenerativeValue = null;

    [SerializeField] private bool verboseLogging = false;
    [SerializeField] private bool superVerboseLogging = false;

    private PlayerBuffs playerBuffs = null;
    private Buffs lastBuff = Buffs.PassivelyRegenerateHP;

    private Weapon activeWeapon = null;
    private ItemData activeTrap = null;
    private Item SelectedItem = null;

    public void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        TryGetComponent(out playerBuffs);

        healthRegenerativeValue.SetCurrentValue(health);
        healthRegenerativeValue.SetMaxValue(maxHealth);
        healthRegenerativeValue.OnChange += SetHealth;

        staminaRegenerativeValue.SetCurrentValue(stamina);
        staminaRegenerativeValue.SetMaxValue(maxStamina);
        staminaRegenerativeValue.OnChange += SetStamina;

        OnPlayerHealthChanged?.Invoke(health);
        OnPlayerStaminaChanged?.Invoke(stamina, maxStamina);

        GameManager.OnNewBuff += OnNewBuff;
    }

    #region Health

    public void DecrementHealth(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(DecrementHealth) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        healthRegenerativeValue.Decrement(amount);
    }

    public void IncrementHealth(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(IncrementHealth) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        healthRegenerativeValue.Increment(amount);
    }

    public void SetHealth(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(SetHealth) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        health = amount;

        if (health == 0)
        {
            OnDie?.Invoke();
        }

        OnPlayerHealthChanged?.Invoke(health);
    }

    #endregion

    #region Stamina

    public void DecrementStamina(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(DecrementStamina) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        staminaRegenerativeValue.Decrement(amount);
    }

    public void IncrementStamina(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(IncrementStamina) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        staminaRegenerativeValue.Increment(amount);
    }

    public void SetStamina(int amount)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(SetStamina) + " ( " + nameof(amount) + ": " + amount + " )", this);
        }

        stamina = amount;

        OnPlayerStaminaChanged?.Invoke(stamina, staminaRegenerativeValue.MaxValue);
    }

    #endregion

    private void OnNewBuff(Buffs newBuff, Buffs nextBuff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnNewBuff) + " ( " + nameof(newBuff) + ": " + newBuff + " , " + nameof(nextBuff) + ": " + nextBuff + " )", this);
        }

        if (newBuff == Buffs.PassivelyRegenerateHP && healthRegenerativeValue.CanStartRegeneration)
        {
            healthRegenerativeValue.StartRegeneration();
        }

        if (lastBuff == Buffs.PassivelyRegenerateHP && healthRegenerativeValue.RegenerationIsActive)
        {
            healthRegenerativeValue.StopRegeneration();
        }

        lastBuff = newBuff;
    }
}