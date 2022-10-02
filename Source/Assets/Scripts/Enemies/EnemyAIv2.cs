using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIv2 : MonoBehaviour
{
    public enum State
    {
        LookingForTarget,
        MovingToTarget,
        AttackingTarget
    }

    private Enemy _enemy;
    private NavMeshAgent _agent;

    private Transform _player;
    private List<Transform> _barricades = new List<Transform>();

    private State _currentState = State.LookingForTarget;
    private Transform _currentTarget;

    private bool _isIndoors = false;
    private bool _isAttackOnCooldown = false;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _agent = GetComponent<NavMeshAgent>();

        _player = GameObject.FindGameObjectWithTag("Player").transform;
        foreach (var barricade in GameObject.FindGameObjectsWithTag("Barricade"))
        {
            _barricades.Add(barricade.transform);
        }
    }

    private void Start()
    {
        // Stopping distance is slightly less so that we can ensure that the enemy is able to attack
        _agent.stoppingDistance = _enemy.attackRange * 0.75f;

        StartCoroutine(UpdateState());
    }

    private void Update()
    {
        if (_currentTarget == null) return;

        if (_enemy.CheckIfObjectIsInRange(_currentTarget))
        {
            var lookPos = _currentTarget.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
        }
    }

    private IEnumerator UpdateState()
    {
        while (true)
        {
            switch (_currentState)
            {
                case State.LookingForTarget:
                    LookForTarget();
                    break;

                case State.MovingToTarget:
                    MoveToTarget();
                    break;

                case State.AttackingTarget:
                    AttackTarget();
                    break;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private void LookForTarget()
    {
        // Find the nearest target based on whether the enemy is indoors or not
        _currentTarget = _isIndoors ? _player : FindNearestBarricade();

        if (_currentTarget != null)
        {
            _currentState = State.MovingToTarget;
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        _agent.SetDestination(_currentTarget.position);

        if (_enemy.CheckIfObjectIsInRange(_currentTarget))
        {
            _currentState = State.AttackingTarget;
            AttackTarget();
        }
    }

    private void AttackTarget()
    {
        if (_isAttackOnCooldown) return;

        if (!_enemy.CheckIfObjectIsInRange(_currentTarget))
        {
            _currentState = State.MovingToTarget;
            MoveToTarget();
        }

        if (_currentTarget == _player)
        {
            // TODO: Attack the player, reduce their HP, etc
        }
        else
        {
            // TODO: Change TestBarricade to Barricade
            if (_currentTarget.gameObject.GetComponent<TestBarricade>().IsDestroyed())
            {
                // TODO: Vault if the barricade is a window

                _isIndoors = true;

                _currentState = State.LookingForTarget;
                LookForTarget();

                return;
            }

            _currentTarget.gameObject.GetComponent<TestBarricade>().GetHit();
            _isAttackOnCooldown = true;

            Invoke(nameof(ResetAttackCooldown), _enemy.attackDelay);
        }

        if (_currentTarget == null)
        {
            _currentState = State.LookingForTarget;
            LookForTarget();
        }
    }

    private void ResetAttackCooldown()
    {
        _isAttackOnCooldown = false;
    }

    private Transform FindNearestBarricade()
    {
        Transform nearestBarricade = _barricades[0];

        for (int i = 0; i < _barricades.Count; i++)
        {
            if (Vector3.Magnitude(_barricades[i].position - transform.position) <
                Vector3.Magnitude(nearestBarricade.position - transform.position))
            {
                nearestBarricade = _barricades[i];
            }
        }

        return nearestBarricade;
    }
}
