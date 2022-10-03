using System;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Item
{
    private const string ENEMY_TAG = "Enemy";

    [SerializeField] private int health = 10;
    [SerializeField] private int damageToEnemies = 20;
    [SerializeField] private int damageWhenHit = 2;
    [SerializeField] private float fireRate = 10;
    [SerializeField] private float lookAtDamping = 5f;
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
    [SerializeField] private bool superVerboseLogging = false;
    [SerializeField] private bool superDuperVerboseLogging = false;

    private bool targetIsObstructed = false;
    private List<Transform> trackedEnemies = new List<Transform>();
    private DateTime nextFire = DateTime.Now;

    private Transform Target => trackedEnemies[0];
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
        LookAtTarget();

        if (DateTime.Now > nextFire && !targetIsObstructed)
        {
            Fire();
            nextFire = DateTime.Now.AddSeconds(fireRate);
        }
    }

    public override void Use(bool held)
    {
        throw new NotImplementedException();
    }

    public void GetHit()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(GetHit), this);
        }

        health -= damageWhenHit;

        if (health <= 0)
        {
            setInactiveOnDeath.SetActive(false);
            setActiveOnDeath.SetActive(true);
            Destroy(gameObject, 5);
        }
    }

    private void LookAtTarget()
    {
        Vector3 targetPosition = new Vector3(Target.position.x, pivot.position.y, Target.position.z);
        Quaternion pivotRotation = Quaternion.LookRotation(targetPosition - pivot.position);
        pivot.rotation = Quaternion.Slerp(pivot.rotation, pivotRotation, Time.deltaTime * lookAtDamping);
    }

    private void Fire()
    {
        if (superDuperVerboseLogging)
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
            hitEnemy.TakeDamage(damageToEnemies);
            Debug.DrawLine(emissionPoint.position, emissionPoint.position + FireDirection * rangeCollider.radius, Color.green, 1f);

            Instantiate(projectile, emissionPoint.position, emissionPoint.rotation);
            emissionParticleSystem.Play();
            emissionAudioSource.Play();
        }
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

        trackedEnemies.Add(colliderTransform);
        CheckIfTargetIsObstructed();
        if (trackedEnemies.Count > 1 && targetIsObstructed)
        {
            SetTargetToUnobstructedEnemy();
        }

        if (!enabled)
        {
            InvokeRepeating(nameof(SetTargetToUnobstructedEnemy), 0, 1);
            enabled = true;
        }
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

        if (trackedEnemies.Count == 0)
        {
            if (enabled)
            {
                CancelInvoke(nameof(SetTargetToUnobstructedEnemy));
                enabled = false;
            }
        }
        else
        {
            SetTargetToUnobstructedEnemy();
        }
    }

    private void CheckIfTargetIsObstructed()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(CheckIfTargetIsObstructed), this);
        }

        targetIsObstructed = false;
        RaycastHit raycastHit;
        if (Physics.Linecast(emissionPoint.position, Target.position, out raycastHit))
        {
            targetIsObstructed = raycastHit.collider.gameObject.tag != ENEMY_TAG;

            if (!targetIsObstructed && verboseLogging)
            {
                Debug.Log(nameof(CheckIfTargetIsObstructed) + $" | {nameof(targetIsObstructed)} = {targetIsObstructed}!", this);
            }
        }
    }

    private void SetTargetToUnobstructedEnemy()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(SetTargetToUnobstructedEnemy), this);
        }

        CheckIfTargetIsObstructed();

        if (!targetIsObstructed || trackedEnemies.Count <= 1)
        {
            return;
        }

        RaycastHit raycastHit;
        for (int i = 1; i < trackedEnemies.Count; i++)
        {
            Transform enemyTransform = trackedEnemies[i];
            if (Physics.Linecast(emissionPoint.position, enemyTransform.position, out raycastHit))
            {
                bool isAnUnobstructedEnemy = raycastHit.collider.gameObject.tag == ENEMY_TAG;

                if (isAnUnobstructedEnemy)
                {
                    Transform newTarget = enemyTransform;
                    trackedEnemies.Remove(enemyTransform);
                    trackedEnemies.Insert(0, enemyTransform);
                    if (superVerboseLogging)
                    {
                        Debug.Log(nameof(SetTargetToUnobstructedEnemy) + $" | found a new target!", this);
                    }
                    break;
                }
                else
                {
                    if (superVerboseLogging)
                    {
                        Debug.Log("Blocked by " + raycastHit.collider.gameObject.tag, this);
                    }
                }
            }
        }

        if (superVerboseLogging)
        {
            Debug.Log(nameof(SetTargetToUnobstructedEnemy) + $" | could not find!", this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(emissionPoint.position, emissionPoint.position + FireDirection * rangeCollider.radius);
    }
}
