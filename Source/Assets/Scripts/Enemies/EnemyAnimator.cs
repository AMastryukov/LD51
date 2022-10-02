using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
