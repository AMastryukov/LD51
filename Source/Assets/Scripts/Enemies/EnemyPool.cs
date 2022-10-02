using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class EnemyPool : NonPersistentSingleton<EnemyPool>
{
    public static Action<GameObject> OnPoolDestroy;

    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int initialPoolSize = 10;
    private Queue<GameObject> objectPool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            LazyInstantiate();
        }
    }

    public void PoolInstantiate(Vector3 position)
    {
        if (objectPool.Count == 0)
        {
            LazyInstantiate();
        }

        GameObject obj = objectPool.Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        EnemyManager.OnEnemySpawned?.Invoke(obj);
    }

    private void LazyInstantiate()
    {
        GameObject obj = Instantiate(objectPrefab, transform);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }

    public void PoolDestroy(GameObject obj)
    {
        OnPoolDestroy?.Invoke(obj);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
        EnemyManager.OnEnemyKilled?.Invoke(obj);
    }
}


