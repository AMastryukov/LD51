using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBarricade : MonoBehaviour, IInteractable
{
    private NavMeshObstacle navMeshObstacle;
    private BoxCollider _collider;
    private MeshRenderer _mesh;

    private int _health = 10;
    private bool _isDestroyed = false;
    private int _max_health;

    private void Awake()
    {
        Debug.LogWarning("We need to consider how rebuilding this barricade affects AI");

        navMeshObstacle = GetComponent<NavMeshObstacle>();
        DebugUtility.HandleErrorIfNullGetComponent(navMeshObstacle, this);

        _collider = GetComponent<BoxCollider>();
        DebugUtility.HandleErrorIfNullGetComponent(_collider, this);

        _mesh = GetComponent<MeshRenderer>();
        DebugUtility.HandleErrorIfNullGetComponent(_mesh, this);

        _max_health = _health;
    }

    public void GetHit()
    {
        _health = Math.Min(_health - 1, 0);
        if (_health == 0)
        {
            SetDestroyed(true);
        }
    }
    public bool IsDestroyed()
    {
        return _isDestroyed;
    }

    public void Interact()
    {
        Debug.Log("Fixed");
        _health = _max_health;
        SetDestroyed(false);
    }

    private void SetDestroyed(bool destroyed)
    {
        navMeshObstacle.enabled = !destroyed;
        _collider.isTrigger = destroyed;
        _isDestroyed = destroyed;
        _mesh.enabled = !destroyed;
    }

    public string GetName()
    {
        throw new NotImplementedException();
    }
}
