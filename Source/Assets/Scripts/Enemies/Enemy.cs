using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int enemyHealth = 10;

    public void TakeDamage()
    {
        Debug.Log("I Took Damage");
    }
}
