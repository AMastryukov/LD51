using System;
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

        _player = GameObject.FindGameObjectWithTag(GameConstants.TagConstants.PlayerTag)?.transform;
        foreach (var barricade in GameObject.FindGameObjectsWithTag(GameConstants.TagConstants.BarricadeTag))
        {
            _barricades.Add(barricade.transform);
        }
    }

    private void OnEnable()
    {
        // Stopping distance is slightly less so that we can ensure that the enemy is able to attack
        _agent.stoppingDistance = _enemy.attackRange * 0.75f;
        _currentState = State.LookingForTarget;
        StartCoroutine(UpdateState());
    }

    private void OnDisable()
    {
        StopCoroutine(UpdateState());
    }

    private void FixedUpdate()
    {
        if (_currentTarget == null) return;

        // RIP Vaulting root motion bug, October 2, 2022 4pm - 5:58pm EST
        if (_enemy.CheckIfObjectIsInRange(_currentTarget) && _currentState != State.Vaulting)
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
        // If out of range, go back to running
        if (!_enemy.CheckIfObjectIsInRange(_currentTarget))
        {
            _animator.Stop();

            _currentState = State.MovingToTarget;
            MoveToTarget();

            return;
        }

        // Lost the target for some reason, go back to looking
        if (_currentTarget == null)
        {
            _animator.Stop();

            _currentState = State.LookingForTarget;
            LookForTarget();
            return;
        }

        // If the target is a barricade and has been destroyed, move through it
        var barricade = _currentTarget.GetComponent<Barricade>();
        if (barricade && barricade.IsDestroyed)
        {
            _animator.Stop();

            MoveThroughBarricade();
            return;
        }

        _animator.Attack();
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
            var barricade = _currentTarget.gameObject.GetComponent<Barricade>();

            if (barricade.IsDestroyed)
            {
                MoveThroughBarricade();
                return;
            }

            barricade.Hit();
        }
    }

    private void MoveThroughBarricade()
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

    private void PerformVault()
    {
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
