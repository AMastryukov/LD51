using System;
using UnityEngine;

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

    private Buffs lastBuff = Buffs.PassivelyRegenerateHP;

    private Weapon activeWeapon = null;
    private ItemData activeTrap = null;
    private Item selectedItem = null;

    public RegenerativeValue StaminaRegenerativeValue => staminaRegenerativeValue;

    private void Awake()
    {
        GameManager.OnNewBuff += OnNewBuff;
    }

    private void OnDestroy()
    {
        GameManager.OnNewBuff -= OnNewBuff;
    }

    public void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        healthRegenerativeValue.SetCurrentValue(health);
        healthRegenerativeValue.SetMaxValue(maxHealth);
        healthRegenerativeValue.OnChange += SetHealth;

        staminaRegenerativeValue.SetCurrentValue(stamina);
        staminaRegenerativeValue.SetMaxValue(maxStamina);
        staminaRegenerativeValue.OnChange += SetStamina;

        OnPlayerHealthChanged?.Invoke(health);
        OnPlayerStaminaChanged?.Invoke(stamina, maxStamina);
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

    private void OnNewBuff(BuffData newBuff, BuffData nextBuff)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnNewBuff) + " ( " + nameof(newBuff.Buff) + ": " + newBuff.Buff + " , " + nameof(nextBuff.Buff) + ": " + nextBuff.Buff + " )", this);
        }

        if (newBuff.Buff == Buffs.PassivelyRegenerateHP &&
            healthRegenerativeValue.CanStartRegeneration &&
            PlayerBuffsManager.Instance.RoomOwned(Buffs.PassivelyRegenerateHP))
        {
            healthRegenerativeValue.StartRegeneration();
        }

        if (lastBuff == Buffs.PassivelyRegenerateHP && healthRegenerativeValue.RegenerationIsActive)
        {
            healthRegenerativeValue.StopRegeneration();
        }

        lastBuff = newBuff.Buff;
    }
}
