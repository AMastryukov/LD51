using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barricade : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip barricadeTakeHit;
    [SerializeField] private AudioClip barricadeBreak;
    [SerializeField] private AudioClip barricadeRepair;

    private AudioSource _audioSource;
    public int FixIncrement => PlayerBuffsManager.Instance.IsBuffActive(Buffs.RepairsAreFaster) ? fixIncrementWithBuff : fixIncrement;
    public bool IsDestroyed { get { return CurrentLevel == 0; } }
    public int CurrentLevel { get; private set; } = 0;
    public int CurrentHealth { get; private set; } = 0;
    public bool IsWindow { get { return isWindow; } }

    [Header("References")]
    [SerializeField] private GameObject[] planks;

    [Header("Values")]
    [SerializeField] private int defaultLevel = 0;
    [SerializeField] private int healthPerLevel = 3;
    [SerializeField] private int fixIncrement = 1;
    [SerializeField] private int fixIncrementWithBuff = 1;
    [SerializeField] private bool isWindow = true;

    private NavMeshObstacle _navMeshObstacle;

    private void Awake()
    {
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        CurrentLevel = defaultLevel;
        CurrentHealth = CurrentLevel > 0 ? healthPerLevel : 0;

        UpdateState();
    }

    public void Hit()
    {
        if (IsDestroyed)
        {
            _audioSource.PlayOneShot(barricadeBreak);
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - 1);

        if (CurrentHealth == 0)
        {
            CurrentLevel = Mathf.Max(0, CurrentLevel - 1);
            CurrentHealth = CurrentLevel > 0 ? healthPerLevel : 0;
        }
        _audioSource.PlayOneShot(barricadeBreak);
        UpdateState();
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        CurrentLevel = Mathf.Min(3, CurrentLevel + 1);
        CurrentHealth = healthPerLevel;

        UpdateState();
        _audioSource.PlayOneShot(barricadeRepair);
    }

    private void UpdateState()
    {
        _navMeshObstacle.enabled = !IsDestroyed;

        // Show planks based on level
        for (int i = 0; i < 3; i++)
        {
            planks[i].SetActive(i < CurrentLevel);
        }
    }

    public string GetName()
    {
        return "Barricade";
    }

    public bool CanInteract()
    {
        return CurrentLevel < 3;
    }
}
