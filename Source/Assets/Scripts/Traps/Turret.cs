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

    private List<Enemy> trackedEnemies = new List<Enemy>();
    private Enemy _currentTarget;
    private float _timeSinceLastShot = 0f;
    private Vector3 FireDirection => emissionPoint.forward;

    private void Awake()
    {
        Enemy.OnEnemyDied += StopTrackingEnemy;
        onTriggerEnterHandler.OnTrigger += OnColliderEntered;
        onTriggerExitHandler.OnTrigger += OnColliderExited;
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDied -= StopTrackingEnemy;
        onTriggerEnterHandler.OnTrigger -= OnColliderEntered;
        onTriggerExitHandler.OnTrigger -= OnColliderExited;
    }

    private void Start()
    {
        Destroy(gameObject, lifespan);

        _timeSinceLastShot = 1f / fireRate;
    }

    private void Update()
    {
        laser.enabled = _currentTarget;

        SearchForTargets();

        if (_currentTarget)
        {
            RotateTowardsCurrentTarget();
            TryFire();
        }

        _timeSinceLastShot += Time.deltaTime;
    }

    private void SearchForTargets()
    {
        if (_currentTarget) return;

        foreach (var enemy in trackedEnemies)
        {
            // Shoot a line cast at every tracked enemy and to find the first one that is unobstructed
            if (Physics.Linecast(transform.position, enemy.transform.position, out RaycastHit hit, hitLayerMask))
            {
                var hitbox = hit.collider.GetComponent<EnemyHitbox>();

                if (hitbox)
                {
                    _currentTarget = hitbox.Owner;
                }
            }
        }
    }

    private void RotateTowardsCurrentTarget()
    {
        if (!_currentTarget) return;

        // Rotate the turret
        Vector3 targetPosition = new Vector3(_currentTarget.transform.position.x, pivot.position.y, _currentTarget.transform.position.z);
        Quaternion pivotRotation = Quaternion.LookRotation(targetPosition - pivot.position);
        pivot.rotation = Quaternion.Slerp(pivot.rotation, pivotRotation, Time.deltaTime * lookAtDamping);

        laser.SetPosition(0, emissionPoint.position);
        laser.SetPosition(1, emissionPoint.position + emissionPoint.transform.forward * rangeCollider.radius);
    }

    private void TryFire()
    {
        if (!_currentTarget) return;
        if (_timeSinceLastShot < 1f / fireRate) return;

        // If we are looking at an enemy hitbox, shoot it
        if (Physics.Raycast(emissionPoint.position, FireDirection, out RaycastHit rayHit, rangeCollider.radius, hitLayerMask))
        {
            var hitbox = rayHit.collider.gameObject.GetComponent<EnemyHitbox>();

            if (hitbox)
            {
                hitbox.TakeDamage(damageToEnemies);
                Debug.DrawLine(emissionPoint.position, emissionPoint.position + FireDirection * rangeCollider.radius, Color.green, 1f);

                Instantiate(projectile, emissionPoint.position, emissionPoint.rotation);

                emissionParticleSystem.Play();
                emissionAudioSource.Play();

                _timeSinceLastShot = 0f;
            }
        }
    }

    private void OnColliderEntered(Collider collider)
    {
        var enemy = collider.GetComponent<Enemy>();

        if (!enemy) return;

        trackedEnemies.Add(enemy);
    }

    private void OnColliderExited(Collider collider)
    {
        var enemy = collider.GetComponent<Enemy>();

        if (!enemy) return;

        StopTrackingEnemy(enemy);
    }

    private void StopTrackingEnemy(Enemy enemy)
    {
        trackedEnemies.Remove(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(emissionPoint.position, emissionPoint.position + FireDirection * rangeCollider.radius);
    }
}