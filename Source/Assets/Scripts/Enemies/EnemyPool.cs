using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
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
    }

    private void LazyInstantiate()
    {
        GameObject obj = Instantiate(objectPrefab, transform);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }

    public void PoolDestroy(GameObject obj)
    {
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }
}


