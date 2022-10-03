using System;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private const string ENEMY_TAG = "Enemy";

    [SerializeField] private int damage = 20;
    [SerializeField] private float fireRate = 10;
    [SerializeField] private SphereCollider rangeCollider = null;
    [SerializeField] private LayerMask hitLayerMask;

    [SerializeField] private Projectile projectile = null;

    [SerializeField] private Transform pivot = null;
    [SerializeField] private Transform emissionPoint = null;
    [SerializeField] private ParticleSystem emissionParticleSystem = null;
    [SerializeField] private AudioSource emissionAudioSource = null;

    [SerializeField] private OnTriggerEnterHandler onTriggerEnterHandler = null;
    [SerializeField] private OnTriggerExitHandler onTriggerExitHandler = null;

    [SerializeField] private GameObject setInactiveOnDeath = null;
    [SerializeField] private GameObject setActiveOnDeath = null;

    [SerializeField] private float timeToDestroyAfterTrigger = 5f;
    [SerializeField] private bool verboseLogging = false;

    private Transform target = null;
    private List<Transform> trackedEnemies = new List<Transform>();
    private DateTime nextFire = DateTime.Now;

    private Vector3 FireDirection => emissionPoint.forward;

    private void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        onTriggerEnterHandler.OnTrigger += OnColliderEntered;
        onTriggerExitHandler.OnTrigger += OnColliderExited;

        EnemyPool.OnPoolDestroy += OnEnemyDespawned;

        enabled = false;
    }

    private void Update()
    {
        pivot.LookAt(target);

        if (DateTime.Now > nextFire)
        {
            Fire();
            nextFire = DateTime.Now.AddSeconds(fireRate);
        }
    }

    private void Fire()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Fire), this);
        }

        RaycastHit rayHit;
        Enemy hitEnemy = null;

        if (Physics.Raycast(emissionPoint.position, FireDirection, out rayHit, rangeCollider.radius, hitLayerMask))
        {
            hitEnemy = rayHit.collider.gameObject.GetComponent<Enemy>();
        }

        if (hitEnemy != null)
        {
            hitEnemy.TakeDamage(damage);
            Debug.DrawLine(emissionPoint.position, emissionPoint.position + FireDirection * rangeCollider.radius, Color.green, 1f);
        }
        else
        {
            Debug.DrawLine(emissionPoint.position, emissionPoint.position + FireDirection * rangeCollider.radius, Color.red, 1f);
        }

        Instantiate(projectile, emissionPoint.position, emissionPoint.rotation);
        emissionParticleSystem.Play();
        emissionAudioSource.Play();
    }

    private void OnColliderEntered(Collider collider)
    {
        Transform colliderTransform = collider.transform;
        if (colliderTransform.tag != ENEMY_TAG)
        {
            return;
        }

        if (verboseLogging)
        {
            Debug.Log(nameof(OnColliderEntered) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        if (trackedEnemies.Count == 0)
        {
            target = colliderTransform;
        }

        trackedEnemies.Add(colliderTransform);

        enabled = true;
    }

    private void OnColliderExited(Collider collider)
    {
        Transform enemyTransform = collider.transform;
        if (enemyTransform.tag != ENEMY_TAG || !trackedEnemies.Contains(enemyTransform))
        {
            return;
        }

        if (verboseLogging)
        {
            Debug.Log(nameof(OnColliderEntered) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        StopTrackingEnemy(enemyTransform);
    }

    private void OnEnemyDespawned(GameObject enemyGameObject)
    {
        Transform enemyTransform = enemyGameObject.transform;

        if (!trackedEnemies.Contains(enemyTransform))
        {
            return;
        }

        if (verboseLogging)
        {
            Debug.Log(nameof(OnEnemyDespawned) + " ( " + nameof(enemyGameObject) + ": " + enemyGameObject.name + " )", this);
        }

        StopTrackingEnemy(enemyTransform);
    }

    private void StopTrackingEnemy(Transform enemyTransform)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(StopTrackingEnemy) + " ( " + nameof(enemyTransform) + ": " + enemyTransform.name + " )", this);
        }

        trackedEnemies.Remove(enemyTransform);

        if (target == enemyTransform && trackedEnemies.Count > 0)
        {
            target = trackedEnemies[0];
        }
        else
        {
            enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(emissionPoint.position, emissionPoint.position + FireDirection * rangeCollider.radius);
    }
}
