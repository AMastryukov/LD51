using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ensure this initializes before EnemyPool
[DefaultExecutionOrder(0)]
public class EnemyManager : NonPersistentSingleton<EnemyManager>
{
    public static Action<GameObject> OnEnemySpawned;
    public static Action<GameObject> OnEnemyKilled;
    private List<EnemyAi> enemies = new List<EnemyAi>();
    public float radiusAroundTarget = 2.5f;

    private void AddEnemyToList(GameObject enemy)
    {
        enemies.Add(enemy.GetComponent<EnemyAi>());
    }

    private void RemoveEnemyFromList(GameObject enemy)
    {
        enemies.Remove(enemy.GetComponent<EnemyAi>());
    }

    private void Awake()
    {
        OnEnemySpawned += AddEnemyToList;
        OnEnemyKilled += RemoveEnemyFromList;
    }
    
    private void OnDestroy()
    {
        OnEnemySpawned -= AddEnemyToList;
        OnEnemyKilled -= RemoveEnemyFromList;
    }

    private void Start()
    {
        StartCoroutine(CircleTarget());
    }

    public IEnumerator CircleTarget()
    {
        while (true)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsInsideRoom)
                {
                    enemies[i].HuntTarget(radiusAroundTarget,i,enemies.Count);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
