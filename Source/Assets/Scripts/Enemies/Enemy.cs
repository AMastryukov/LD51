using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public const string TAG = "Enemy";

    public enum BodyPart { Head, Body, LeftArm, RightArm, LeftLeg, RightLeg }

    public int enemyHealth = 10;
    public float attackRange = 2f;
    public float attackDelay = 2f;


    [Header("HitBoxes")]
    [SerializeField] private EnemyHitbox[] hitBoxes;

    private void Awake()
    {
        foreach (var hitbox in hitBoxes)
        {
            hitbox.AssignOwner(this);
        }
    }

    public bool CheckIfObjectIsInRange(Transform obj)
    {
        return Vector3.Magnitude(Vector3.ProjectOnPlane(obj.position - transform.position, Vector3.up)) < attackRange;
    }

    public void TakeDamage(int damage, BodyPart bodyPart = BodyPart.Body)
    {
        Debug.Log("I Took" + damage + " Damage");
        Debug.Log("I Have" + enemyHealth + " Health");
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            EnemyPool.Instance.PoolDestroy(gameObject);

            // TODO: Implement actual death
        }
    }
}
