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

    // TODO: Call this in Weapon.cs by raycasting for EnemyHitbox instead of just Enemy
    public void TakeDamage(int damage)
    {
        if (Owner == null)
        {
            Debug.LogError($"Hitbox owner is not set on {gameObject.name}. \nEnsure that all hitboxes are tracked by the Enemy script");
            return;
        }

        // Apply any damage modifiers here

        Owner.TakeDamage(damage, bodyPart);
    }
}
