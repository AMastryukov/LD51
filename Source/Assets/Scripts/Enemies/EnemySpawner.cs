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
    [SerializeField] private Enemy enemyPrefab;

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
        SpawnEnemyAtSpecificSpawnPoint(randomIndex);
    }

    private void SpawnEnemyAtSpecificSpawnPoint(int spawnPointIndex)
    {
        if (spawnPointIndex >= enemySpawnPoints.Count)
        {
            Debug.LogError("Spawn index out of bounds!");
            return;
        }

        Instantiate(enemyPrefab, enemySpawnPoints[spawnPointIndex]);
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


