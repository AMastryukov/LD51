using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private EnemyAiState _currentState = EnemyAiState.OutsideHouse;

    private NavMeshAgent _agent;
    private Transform _player;
    private List<Transform> _barricades = new List<Transform>();
    private List<Transform> _turrets = new List<Transform>();
    private Transform _nearestTurret;
    private Transform _target;

    private Enemy _enemy;
    private bool _isAttackOnCooldown;
    private bool _isTargetTurret = false;
    private bool _isInsideRoom = false;
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

                    // Don't stand around waiting if barricade is already destroyed
                    if (_currentState == EnemyAiState.OutsideHouse)
                    {
                        yield return new WaitForSeconds(_enemy.attackDelay);
                    }

                    break;

                case EnemyAiState.SearchForTarget:
                    FindTarget();
                    if (_target != null)
                    {
                        _currentState = EnemyAiState.HuntingTarget;
                    }
                    else
                    {
                        // No turrets or players could be found in the scene so the loop can stop
                        Debug.Log("No targets");
                        running = false;
                    }
                    break;

                case EnemyAiState.HuntingTarget:
                    if (!_isInsideRoom && _target != null)
                    {
                        var barricade = _target.GetComponent<Barricade>();

                        if (barricade && barricade.IsDestroyed)
                        {
                            _currentState = EnemyAiState.OutsideHouse;
                            MoveToNearestBarricade();
                        }
                    }

                    HuntTarget();

                    yield return new WaitForSeconds(_isTargetTurret ? _enemy.attackDelay : 0.25f);
                    break;
            }
        }
    }

    private void Update()
    {
        if (_enemy.CheckIfObjectIsInRange(_target))
        {
            var lookPos = _target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
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
        // Stopping distance is slightly less so that we can ensure that the enemy is able to attack
        _agent.stoppingDistance = _enemy.attackRange * 0.75f;

        FindNearestBarricade();

        if (_target == null)
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
        //If target dies, change search for next target
        if (_target == null)
            _currentState = EnemyAiState.SearchForTarget;
    }

    private void AttackBarricade()
    {
        if (_target.gameObject.GetComponent<Barricade>().IsDestroyed)
        {
            _currentState = EnemyAiState.SearchForTarget;
            FindNearestTurret();

            return;
        }

        transform.LookAt(_target);

        if (_isAttackOnCooldown) return;

        _target.gameObject.GetComponent<Barricade>().Hit();
        _isAttackOnCooldown = true;

        Invoke(nameof(ResetAttack), _enemy.attackDelay);
    }

    private void ResetAttack()
    {
        _isAttackOnCooldown = false;
    }
    #endregion

    #region Traversal
    private void AttackBarricadeIfInRange()
    {
        if (_enemy.CheckIfObjectIsInRange(_target))
        {
            AttackBarricade();
        }
        else
        {
            MoveToNearestBarricade();
        }
    }

    private void MoveToNearestBarricade()
    {
        _agent.SetDestination(_target.position);
    }

    private void HuntTarget()
    {
        if (_enemy.CheckIfObjectIsInRange(_target))
        {
            AttackTarget();
            return;
        }

        Debug.LogWarning("This can throw an error when the character isn't ont the nav mesh");
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

    public void SetRoomState(bool state)
    {
        _isInsideRoom = state;
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
                _target = _barricades[i];
            }
            else
            {
                if (Vector3.Magnitude(_barricades[i].position - transform.position) <
                    Vector3.Magnitude(_target.position - transform.position))
                {
                    _target = _barricades[i];
                }
            }
        }
    }
    #endregion
}
