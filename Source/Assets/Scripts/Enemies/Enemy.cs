using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum BodyPart { Head, Body, LeftArm, RightArm, LeftLeg, RightLeg }

    public int enemyHealth = 10;
    public float attackRange = 2f;
    public float attackDelay = 2f;

    public bool CheckIfObjectIsInRange(Transform obj)
    {
        return Vector3.Magnitude(obj.position - transform.position) < attackRange;
    }

    public void TakeDamage(int damage, BodyPart bodyPart = BodyPart.Body)
    {
        Debug.Log("I Took" + damage + " Damage");
        Debug.Log("I Have" + enemyHealth + " Health");
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            Destroy(gameObject);

            // TODO: Implement actual death
        }
    }
}
