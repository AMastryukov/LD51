using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarbedWire : MonoBehaviour
{
    [SerializeField] private int health = 20;
    [SerializeField] [Tooltip("In seconds")] private float damageFrequency = 1f;
    [SerializeField] private int damageToEnemies = 1;
    [SerializeField] private int damageWhenHit = 2;

    [SerializeField] private OnTriggerEnterHandler onTriggerEnterHandler = null;
    [SerializeField] private OnTriggerExitHandler onTriggerExitHandler = null;

    [SerializeField] private GameObject setInactiveOnDeath = null;
    [SerializeField] private GameObject setActiveOnDeath = null;

    [SerializeField] private float timeToDestroyAfterTrigger = 5f;

    [SerializeField] private bool verboseLogging = false;

    private List<Enemy> trackedEnemies = new List<Enemy>();

    private DateTime nextOuchTime = DateTime.Now;

    private void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        onTriggerEnterHandler.OnTrigger += OnColliderEntered;
        onTriggerExitHandler.OnTrigger += OnColliderExited;
        EnemyPool.OnPoolDestroy += OnEnemyDespawned;

        enabled = false;
    }

    private void Update()
    {
        if (DateTime.Now > nextOuchTime)
        {
            HurtTrackedEnemies();
        }
    }

    public void GetHit()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(GetHit), this);
        }

        health -= damageWhenHit;

        if (health <= 0)
        {
            setInactiveOnDeath.SetActive(false);
            setActiveOnDeath.SetActive(true);
            Destroy(gameObject, 5);
        }
    }

    private void HurtTrackedEnemies()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(HurtTrackedEnemies), this);
        }

        for (var i = trackedEnemies.Count; i-- > 0;)
        {
            Enemy enemyToHurt = trackedEnemies[i];
            if (enemyToHurt != null)
            {
                enemyToHurt.TakeDamage(damageToEnemies);
            }
            else
            {
                trackedEnemies.RemoveAt(i);
            }
            


        }

        nextOuchTime = DateTime.Now.AddSeconds(damageFrequency);
    }

    private void OnColliderEntered(Collider collider)
    {
        Transform colliderTransform = collider.transform;
        if (!colliderTransform.CompareTag(GameConstants.TagConstants.EnemyTag))
        {
            return;
        }

        if (verboseLogging)
        {
            Debug.Log(nameof(OnColliderEntered) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        colliderTransform.TryGetComponent(out Enemy enemy);
        trackedEnemies.Add(enemy);

        colliderTransform.TryGetComponent(out EnemyAIv2 enemyAi);
        enemyAi.ToggleSpeed(true);

        enabled = true;
    }

    private void OnColliderExited(Collider collider)
    {
        Transform enemyTransform = collider.transform;
        if (!enemyTransform.CompareTag(GameConstants.TagConstants.EnemyTag))
        {
            return;
        }

        enemyTransform.TryGetComponent(out Enemy enemy);
        if (!trackedEnemies.Contains(enemy))
        {
            return;
        }

        if (verboseLogging)
        {
            Debug.Log(nameof(OnColliderExited) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        StopTrackingEnemy(enemy);
    }

    private void OnEnemyDespawned(GameObject enemyGameObject)
    {
        enemyGameObject.TryGetComponent(out Enemy enemy);

        if (!trackedEnemies.Contains(enemy))
        {
            return;
        }

        if (verboseLogging)
        {
            Debug.Log(nameof(OnEnemyDespawned) + " ( " + nameof(enemyGameObject) + ": " + enemyGameObject.name + " )", this);
        }

        StopTrackingEnemy(enemy);
    }

    private void StopTrackingEnemy(Enemy enemy)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(StopTrackingEnemy) + " ( " + nameof(enemy) + ": " + enemy.name + " )", this);
        }

        enemy.TryGetComponent(out EnemyAIv2 enemyAi);
        enemyAi.ToggleSpeed(false);

        trackedEnemies.Remove(enemy);

        if (trackedEnemies.Count == 0)
        {
            if (enabled)
            {
                enabled = false;
            }
        }
    }
}
