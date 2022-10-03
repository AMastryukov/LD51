using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    #region Fields

    [SerializeField] private List<Transform> enemySpawnPoints;
    [SerializeField] private int enemyIncrementPerWave = 1;
    [SerializeField] private int enemiesToSpawn = 5;

    #endregion

    #region SubscriptionHandling

    private void Awake()
    {
        GameManager.OnTenSecondsPassed += SpawnEnemyWave;
    }

    private void OnDestroy()
    {
        GameManager.OnTenSecondsPassed -= SpawnEnemyWave;
    }

    #endregion

    private void Start()
    {

        SpawnEnemyWave();
    }

    #region SpawnerFunctions

    private void SpawnEnemyAtRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, enemySpawnPoints.Count);
        Debug.Log($"spawning enemy at {enemySpawnPoints[randomIndex].position}");
        EnemyPool.Instance.PoolInstantiate(enemySpawnPoints[randomIndex].position);
    }

    private void SpawnEnemyAtSpecificSpawnPoint(int spawnPointIndex)
    {
        if (spawnPointIndex >= enemySpawnPoints.Count)
        {
            Debug.LogError("Index out of bounds!");
            return;
        }

        EnemyPool.Instance.PoolInstantiate(enemySpawnPoints[spawnPointIndex].position);
    }

    private void SpawnEnemyWave()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemyAtRandomSpawnPoint();
        }

        //Increase amount to spawn for the next wave
        enemiesToSpawn += enemyIncrementPerWave;
    }

    #endregion

}


