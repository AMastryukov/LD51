using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] private Enemy.BodyPart bodyPart = Enemy.BodyPart.Body;
    public Enemy Owner { get; private set; }

    public void AssignOwner(Enemy owner)
    {
        Owner = owner;
    }

    public void TakeDamage(int damage)
    {
        if (Owner == null) return;
        Owner.TakeDamage(damage, bodyPart);
    }
}
