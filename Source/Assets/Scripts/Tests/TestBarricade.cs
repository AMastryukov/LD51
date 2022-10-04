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

    [SerializeField] private List<Material> levelMaterials;
    [SerializeField] private int fixIncrement = 1;
    [SerializeField] private int fixIncrementWithBuff = 1;

    public int FixIncrement => PlayerBuffsManager.Instance.IsBuffActive(Buffs.RepairsAreFaster) ? fixIncrementWithBuff : fixIncrement;

    private int _health = 100;
    private bool _isDestroyed = false;
    private int _max_health;
    private int _level;
    private const int MaxLevel = 3;

    private void Awake()
    {
        Debug.LogWarning("We need to consider how rebuilding this barricade affects AI");

        navMeshObstacle = GetComponent<NavMeshObstacle>();
        DebugUtility.HandleErrorIfNullGetComponent(navMeshObstacle, this);

        _collider = GetComponent<BoxCollider>();
        DebugUtility.HandleErrorIfNullGetComponent(_collider, this);

        _mesh = GetComponent<MeshRenderer>();
        DebugUtility.HandleErrorIfNullGetComponent(_mesh, this);

        if (levelMaterials.Count != MaxLevel)
            Debug.LogError("AssignMaterials PLEASE");
        SetLevel(1);
    }

    public void GetHit()
    {
        Debug.Log("Barricade got hit!");
        _health -= 25;
        if (_health <= 0)
        {
            SetLevel(_level - 1);
        }
    }
    public bool IsDestroyed()
    {
        return _isDestroyed;
    }

    public void Interact()
    {
        Debug.Log("Fixed");
        SetDestroyed(false);
        if (_level < MaxLevel)
            SetLevel(_level + FixIncrement);
    }

    private void SetDestroyed(bool destroyed)
    {
        navMeshObstacle.enabled = !destroyed;
        _collider.isTrigger = destroyed;
        _isDestroyed = destroyed;
        _mesh.enabled = !destroyed;

        Debug.Log("Barricade was destroyed");
    }

    public string GetName()
    {
        throw new NotImplementedException();
    }


    private void SetLevel(int level)
    {
        _level = level;
        if (level <= 0)
        {
            SetDestroyed(true);
            return;
        }
        //_mesh.material = levelMaterials[_level - 1];
        Debug.Log("Level: " + _level);
        _health = _max_health;
    }

    public bool CanInteract()
    {
        throw new NotImplementedException();
    }
}
