using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Katana : Weapon
{
    private List<Enemy> _enemiesInRange = new List<Enemy>();
    

    protected override void Fire()
    {
        lastFireTime = Time.time;
        audioSource.Play();
        muzzlePasticleSystem.Play();
        AccumulateRecoil(recoilAmount);
        //transform.DOPunchPosition(transform.forward, 0.25f);
        if(_enemiesInRange.Count==0)
            return;
        
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
