using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyHealth = 10;
    public float attackRange = 3f;
    public float attacksPerSecond = 4f;
    
    public bool CheckIfObjectIsInRange(Transform obj)
    {
        return Vector3.Magnitude(obj.position - transform.position) < attackRange;
    }
    
    public void TakeDamage()
    {
        Debug.Log("I Took Damage");
    }
}
