using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    #region Fields

    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private List<Transform> enemySpawnPoints;
    [SerializeField] private int enemyIncrementPerWave = 1;
    private int _enemiesToSpawn = 5;

    #endregion

    #region SubscriptionHandling

    private void OnEnable()
    {
        //Subscribe
    }

    private void OnDisable()
    {
        //UnSubscribe
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
        enemyPool.PoolInstantiate(enemySpawnPoints[randomIndex].position);
    }

    private void SpawnEnemyAtSpecificSpawnPoint(int spawnPointIndex)
    {
        if (spawnPointIndex >= enemySpawnPoints.Count)
        {
            Debug.LogError("Index out of bounds!");
            return;
        }

        enemyPool.PoolInstantiate(enemySpawnPoints[spawnPointIndex].position);
    }

    private void SpawnEnemyWave()
    {
        for (int i = 0; i < _enemiesToSpawn; i++)
        {
            SpawnEnemyAtRandomSpawnPoint();
        }

        //Increase amount to spawn for the next wave
        _enemiesToSpawn += enemyIncrementPerWave;
    }

    #endregion

}


