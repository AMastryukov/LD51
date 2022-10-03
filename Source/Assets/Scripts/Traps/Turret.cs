using System;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SphereCollider rangeCollider = null;
    [SerializeField] private LayerMask hitLayerMask;

    [SerializeField] private Projectile projectile = null;
    [SerializeField] private LineRenderer laser;

    [SerializeField] private Transform pivot = null;
    [SerializeField] private Transform emissionPoint = null;
    [SerializeField] private ParticleSystem emissionParticleSystem = null;
    [SerializeField] private AudioSource emissionAudioSource = null;

    [SerializeField] private OnTriggerEnterHandler onTriggerEnterHandler = null;
    [SerializeField] private OnTriggerExitHandler onTriggerExitHandler = null;

    [Header("Gameplay")]
    [SerializeField] private int damageToEnemies = 20;
    [SerializeField] private float fireRate = 10;
    [SerializeField] private float lookAtDamping = 5f;
    [SerializeField] private float lifespan = 10f;

    [Header("Debugging")]
    [SerializeField] private bool verboseLogging = false;
    [SerializeField] private bool superVerboseLogging = false;
    [SerializeField] private bool superDuperVerboseLogging = false;

    private bool targetIsObstructed = false;
    private List<Enemy> trackedEnemies = new List<Enemy>();
    private float _timeSinceLastShot = 0f;

    private Transform CurrentTarget => trackedEnemies[0].transform;
    private Vector3 FireDirection => emissionPoint.forward;

    private void Awake()
    {
        Enemy.OnEnemyDied += StopTrackingEnemy;

        onTriggerEnterHandler.OnTrigger += OnColliderEntered;
        onTriggerExitHandler.OnTrigger += OnColliderExited;

        _timeSinceLastShot = 1f / fireRate;
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDied -= StopTrackingEnemy;

        onTriggerEnterHandler.OnTrigger -= OnColliderEntered;
        onTriggerExitHandler.OnTrigger -= OnColliderExited;
    }

    private void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        Destroy(gameObject, lifespan);
    }

    private void Update()
    {
        if (trackedEnemies.Count == 0)
        {
            if (laser.enabled)
            {
                laser.enabled = false;
            }

            return;
        }

        LookAtTarget();
        Fire();

        _timeSinceLastShot += Time.deltaTime;
    }

    private void LookAtTarget()
    {
        if (CurrentTarget == null) return;

        Vector3 targetPosition = new Vector3(CurrentTarget.position.x, pivot.position.y, CurrentTarget.position.z);
        Quaternion pivotRotation = Quaternion.LookRotation(targetPosition - pivot.position);
        pivot.rotation = Quaternion.Slerp(pivot.rotation, pivotRotation, Time.deltaTime * lookAtDamping);

        if (!laser.enabled)
        {
            laser.enabled = true;
        }

        laser.SetPosition(0, emissionPoint.position);
        laser.SetPosition(1, emissionPoint.position + emissionPoint.transform.forward * rangeCollider.radius);
    }

    private void Fire()
    {
        if (_timeSinceLastShot < 1f / fireRate) return;

        if (superDuperVerboseLogging)
        {
            Debug.Log(nameof(Fire), this);
        }

        RaycastHit rayHit;
        EnemyHitbox hitEnemy = null;

        if (Physics.Raycast(emissionPoint.position, FireDirection, out rayHit, rangeCollider.radius, hitLayerMask))
        {
            hitEnemy = rayHit.collider.gameObject.GetComponent<EnemyHitbox>();
        }

        if (hitEnemy != null)
        {
            hitEnemy.TakeDamage(damageToEnemies);
            Debug.DrawLine(emissionPoint.position, emissionPoint.position + FireDirection * rangeCollider.radius, Color.green, 1f);

            Instantiate(projectile, emissionPoint.position, emissionPoint.rotation);

            emissionParticleSystem.Play();
            emissionAudioSource.Play();

            _timeSinceLastShot = 0f;
        }
    }

    private void OnColliderEntered(Collider collider)
    {
        Transform colliderTransform = collider.transform;
        var enemy = colliderTransform.GetComponent<Enemy>();

        if (!enemy)
        {
            return;
        }

        if (verboseLogging)
        {
            Debug.Log(nameof(OnColliderEntered) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        trackedEnemies.Add(enemy);

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
        Transform colliderTransform = collider.transform;
        var enemy = colliderTransform.GetComponent<Enemy>();

        if (!enemy || !trackedEnemies.Contains(enemy))
        {
            return;
        }

        if (verboseLogging)
        {
            Debug.Log(nameof(OnColliderExited) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        StopTrackingEnemy(enemy);
    }

    private void StopTrackingEnemy(Enemy enemy)
    {
        trackedEnemies.Remove(enemy);

        if (trackedEnemies.Count == 0)
        {
            if (enabled)
            {
                CancelInvoke(nameof(SetTargetToUnobstructedEnemy));
                if (laser.enabled)
                {
                    laser.enabled = false;
                }
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
        if (Physics.Linecast(emissionPoint.position, CurrentTarget.position, out raycastHit))
        {
            targetIsObstructed = !raycastHit.collider.gameObject.CompareTag(GameConstants.TagConstants.EnemyTag);

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
            Transform enemyTransform = trackedEnemies[i].transform;
            if (Physics.Linecast(emissionPoint.position, enemyTransform.position, out raycastHit))
            {
                Enemy enemy = raycastHit.collider.GetComponent<Enemy>();
                bool isAnUnobstructedEnemy = enemy;

                if (isAnUnobstructedEnemy)
                {
                    Transform newTarget = enemyTransform;

                    trackedEnemies.Remove(enemy);
                    trackedEnemies.Insert(0, enemy);

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
