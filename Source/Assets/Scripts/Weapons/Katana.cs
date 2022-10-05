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

        Invoke(nameof(AttackAllEnemiesInRange), 0.12f);


    }

    private void AttackAllEnemiesInRange()
    {
        RaycastHit[] hits = Physics.SphereCastAll(_playerCamera.position, 2, _playerCamera.forward, 1);
        foreach (RaycastHit hit in hits)
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                if (bloodParticleSystem != null)
                {
                    Destroy(Instantiate(bloodParticleSystem, hit.point, Quaternion.identity).gameObject, 2);

                }
            }

        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //if (!other.CompareTag(GameConstants.TagConstants.EnemyTag))
    //    //{
    //    //    return;
    //    //}

    //    Enemy enemyRef = other.GetComponent<Enemy>();
    //    if (enemyRef != null)
    //    {
    //        _enemiesInRange.Add(enemyRef);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    //if (!other.CompareTag(GameConstants.TagConstants.EnemyTag))
    //    //{
    //    //    return;
    //    //}
    //    Enemy enemyRef = other.GetComponent<Enemy>();
    //    if (enemyRef != null && _enemiesInRange.Contains(enemyRef))
    //    {
    //        _enemiesInRange.Remove(enemyRef);
    //    }

    //}
}
