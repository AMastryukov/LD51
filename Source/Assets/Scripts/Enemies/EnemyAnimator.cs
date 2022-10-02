using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;

    private float _maxSpeed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _maxSpeed = _agent.speed;
    }

    private void Update()
    {
        _animator?.SetFloat("movementSpeed", _agent.velocity.magnitude / _maxSpeed);
    }

    public void Vault()
    {
        Stop();
        _animator?.SetBool("isVaulting", true);
    }

    public void Attack()
    {
        Stop();
        _animator?.SetBool("isAttacking", true);
    }

    public void Run()
    {
        Stop();
        _animator?.SetFloat("movementSpeed", 1f);
    }

    public void Dance()
    {
        Stop();
        _animator?.SetBool("isDancing", true);
    }

    public void Stop()
    {
        _animator?.SetBool("isAttacking", false);
        _animator?.SetBool("isVaulting", false);
        _animator?.SetBool("isDancing", false);

        _animator?.SetFloat("movementSpeed", 0f);
    }

    public void OnVaultFinished()
    {
        _animator?.SetBool("isVaulting", false);
    }
}
