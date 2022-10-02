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
        Vaulting,
        AttackingTarget
    }

    private Enemy _enemy;
    private NavMeshAgent _agent;
    private EnemyAnimator _animator;

    private Transform _player;
    private List<Transform> _barricades = new List<Transform>();

    private State _currentState = State.LookingForTarget;
    private Transform _currentTarget;

    private bool _isIndoors = false;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<EnemyAnimator>();

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

    private void FixedUpdate()
    {
        if (_currentTarget == null) return;

        if (_enemy.CheckIfObjectIsInRange(_currentTarget))
        {
            var lookPos = _currentTarget.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
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

                case State.Vaulting:
                    Debug.Log("Vaulting atm");
                    break;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private void LookForTarget()
    {
        // Find the nearest target based on whether the enemy is indoors or not
        _currentTarget = _isIndoors ? _player : FindNearestBarricade();

        _animator.Stop();

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
        if (!_enemy.CheckIfObjectIsInRange(_currentTarget))
        {
            _animator.Stop();

            _currentState = State.MovingToTarget;
            MoveToTarget();

            return;
        }

        _animator.Attack();

        if (_currentTarget == null)
        {
            _currentState = State.LookingForTarget;
            LookForTarget();
        }
    }

    /// <summary>
    /// This is called from an animation event to make it more accurate
    /// </summary>
    public void PerformHit()
    {
        if (_currentTarget == _player)
        {
            // TODO: Attack the player, reduce their HP, etc
        }
        else
        {
            _currentTarget.gameObject.GetComponent<TestBarricade>().GetHit();

            // TODO: Change TestBarricade to Barricade
            if (_currentTarget.gameObject.GetComponent<TestBarricade>().IsDestroyed())
            {
                // TODO: if Barricade is a window
                bool isWindow = true;
                if (isWindow)
                {
                    // Perform the vault if near an unbarricated window
                    _currentState = State.Vaulting;
                    PerformVault();

                    return;
                }
                else
                {
                    // If at an unbarricated doorway, move in and start looking for targets
                    _currentState = State.LookingForTarget;
                    LookForTarget();

                    return;
                }
            }
        }
    }

    private void PerformVault()
    {
        _animator.Stop();
        _animator.Vault();
    }

    public void OnVaultingFinished()
    {
        _isIndoors = true;
        _animator.Stop();

        _currentState = State.LookingForTarget;
        LookForTarget();
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
