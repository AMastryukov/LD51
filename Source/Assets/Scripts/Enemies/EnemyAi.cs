using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum EnemyAiState 
{
    OutsideHouse,
    SearchForTarget,
    HuntingTarget,
}

public class EnemyAi : MonoBehaviour
{
    #region Fields

    private NavMeshAgent _agent;
    private Transform _player;
    private List<Transform> _barricades = new List<Transform>();
    private List<Transform> _turrets = new List<Transform>();
    private Transform _nearestBarricade;
    private Transform _nearestTurret;
    private Transform _target;

    private Enemy _enemy;
    private EnemyAiState _currentState = EnemyAiState.OutsideHouse;
    private bool _isAttackOnCooldown;
    private bool _isTargetTurret = false;


    #endregion


    private IEnumerator EnemyAiUpdateLoop()
    {
        bool running = true;
        while (running)
        {
            switch (_currentState)
            {
                case EnemyAiState.OutsideHouse:
                    AttackBarricadeIfInRange();
                    yield return new WaitForSeconds(_enemy.attacksPerSecond);
                    break;
                case EnemyAiState.SearchForTarget:
                    FindTarget();
                    if (_target != null)
                    {
                        _currentState = EnemyAiState.HuntingTarget;
                    }
                    else
                    {
                        //No turrets or players could be found in the scene so the loop can stop
                        Debug.Log("No targets");
                        running = false;
                    }
                    break;
                case EnemyAiState.HuntingTarget:
                    HuntTarget();
                    yield return new WaitForSeconds(_isTargetTurret? _enemy.attacksPerSecond : 0.25f);
                    break;
            }
        }
    }

    #region Initialization

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        PopulateTargetLists();
    }

    private void Start()
    {
        FindNearestBarricade();
        if (_nearestBarricade == null)
        {
            Debug.LogError("No objects with tag barricade found!");
            return;
        }
        MoveToNearestBarricade();
        StartCoroutine(EnemyAiUpdateLoop());
    }
    private void PopulateTargetLists()
    {
        GameObject[] barricadeObjects = GameObject.FindGameObjectsWithTag("Barricade");
        foreach (GameObject obj in barricadeObjects)
        {
            _barricades.Add(obj.transform);
        }

        GameObject[] turretObjects = GameObject.FindGameObjectsWithTag("Turret");
        foreach (GameObject obj in turretObjects)
        {
            _turrets.Add(obj.transform);
        }
    }


    #endregion
    
    #region Attack

    private void AttackTarget()
    {
        //Add functionality here
        transform.LookAt(_nearestBarricade);

        //If target dies, change search for next target
        if (_target == null)
            _currentState = EnemyAiState.SearchForTarget;
    }

    private void AttackBarricade()
    {
        if (_nearestBarricade.gameObject.GetComponent<TestBarricade>().IsDestroyed())
        {
            _currentState = EnemyAiState.SearchForTarget;
            FindNearestTurret();
            return;
        }

        transform.LookAt(_nearestBarricade);
        if (_isAttackOnCooldown) return;
        _nearestBarricade.gameObject.GetComponent<TestBarricade>().GetHit();
        _isAttackOnCooldown = true;
        Invoke(nameof(ResetAttack), _enemy.attacksPerSecond);
    }

    private void ResetAttack()
    {
        _isAttackOnCooldown = false;
    }

    #endregion

    #region Traversal

    private void AttackBarricadeIfInRange()
    {
        if (_enemy.CheckIfObjectIsInRange(_nearestBarricade))
        {
            AttackBarricade();
        }
    }

    private void MoveToNearestBarricade()
    {
        _agent.SetDestination(_nearestBarricade.position);
    }

    private void HuntTarget()
    {
        if (_enemy.CheckIfObjectIsInRange(_target))
        {
            AttackTarget();
            return;
        }
        
        _agent.SetDestination(_target.position);
    }

    #endregion
    
    #region Search

    private void FindTarget()
    {
        if (_nearestTurret == null)
        {
            _target = _player;
        }
        else
        {
            if (_player == null)
                _target = _nearestTurret;

            _target = Vector3.Magnitude(_nearestTurret.position - transform.position) <
                      Vector3.Magnitude(_player.position - transform.position)
                ? _nearestTurret
                : _player;
        }

        _isTargetTurret = _target == _nearestTurret;
    }

    private void FindNearestTurret()
    {
        for (int i = 0; i < _turrets.Count; i++)
        {
            if (i == 0)
            {
                _nearestTurret = _barricades[i];
            }
            else
            {
                if (Vector3.Magnitude(_turrets[i].position - transform.position) <
                    Vector3.Magnitude(_nearestTurret.position - transform.position))
                {
                    _nearestTurret = _barricades[i];
                }
            }
        }
    }

    private void FindNearestBarricade()
    {
        for (int i = 0; i < _barricades.Count; i++)
        {
            if (i == 0)
            {
                _nearestBarricade = _barricades[i];
            }
            else
            {
                if (Vector3.Magnitude(_barricades[i].position - transform.position) <
                    Vector3.Magnitude(_nearestBarricade.position - transform.position))
                {
                    _nearestBarricade = _barricades[i];
                }
            }
        }
    }

    #endregion
    
   
}