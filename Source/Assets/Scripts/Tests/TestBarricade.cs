using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBarricade : MonoBehaviour
{
    [SerializeField] private NavMeshObstacle navMeshObstacle;
    private BoxCollider _collider;
    
    private int _health = 10;
    private bool _isDestroyed = false;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    public void GetHit()
    {
        _health--;
        if (_health == 0)
        {
            navMeshObstacle.enabled = false;
            _collider.enabled = false;
            _isDestroyed = true;
        }
    }
    public bool IsDestroyed()
    {
        return _isDestroyed;
    }
}
