using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyHealth = 10;
    public float attackRange = 2f;
    public float attackDelay = 2f;

    public bool CheckIfObjectIsInRange(Transform obj)
    {
        return Vector3.Magnitude(obj.position - transform.position) < attackRange;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("I Took" + damage + " Damage");
        Debug.Log("I Have" + enemyHealth + " Health");
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            // TODO: Implement actual death
            Destroy(gameObject);
        }
    }
}
