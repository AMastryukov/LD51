using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Katana : Weapon
{
    private List<Enemy> _enemiesInRange = new List<Enemy>();
    [SerializeField] private Animator _animator;
    

    protected override void Fire()
    {
        lastFireTime = Time.time;
        audioSource.Play();
        _animator.SetTrigger("Attack");
        
        if(_enemiesInRange.Count==0)
            return;
        
       Invoke(nameof(AttackAllEnemiesInRange),0.12f);
    }

    private void AttackAllEnemiesInRange()
    {
        foreach (Enemy enemy in _enemiesInRange)
        {
            if (enemy == null)
            {
                _enemiesInRange.Remove(enemy);
                return;
            }
            enemy.TakeDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(GameConstants.TagConstants.EnemyTag))
        {
            return;
        }

        Enemy enemyRef = other.GetComponent<Enemy>();
        if (enemyRef != null)
        {
            _enemiesInRange.Add(enemyRef);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(GameConstants.TagConstants.EnemyTag))
        {
            return;
        }
        Enemy enemyRef = other.GetComponent<Enemy>();
        if (enemyRef != null && _enemiesInRange.Contains(enemyRef))
        {
            _enemiesInRange.Remove(enemyRef);
        }
        
    }
}
