using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum BodyPart { Head, Body, LeftArm, RightArm, LeftLeg, RightLeg }

    public int enemyHealth = 10;
    public float attackRange = 2f;
    public float attackDelay = 2f;

    [Header("HitBoxes")]
    [SerializeField] private EnemyHitbox[] hitBoxes;

    private EnemyCorpse _corpse;
    private Component[] _components;

    private void Awake()
    {
        _corpse = GetComponent<EnemyCorpse>();
        _components = GetComponents<Component>();

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
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            _corpse.ExplodeBodyPart(bodyPart);

            foreach (var component in _components)
            {
                if (component != this &&
                    component != _corpse &&
                    component != transform)
                {
                    Destroy(component);
                }
            }

            Destroy(this);
        }
    }
}
