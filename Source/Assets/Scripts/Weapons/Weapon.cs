using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireType
{
    SINGLE,
    AUTO
}
public class Weapon : Item
{
    [Header("Weapon")]
    [SerializeField]
    FireType weaponFireType = FireType.SINGLE;
    [SerializeField]
    private int damage = 5;
    [SerializeField]
    private float fireRate = 5;
    private float lastFireTime = 0;
    [SerializeField]
    private int shotCount = 1;
    [Range(0f, 1f)]
    [SerializeField]
    private float spread = 0;
    [SerializeField]
    [Tooltip("Damage will be 0 at the end of the range")]
    private float range = 10f;
    [Tooltip("Damage will fall off linearly over the range")]
    [SerializeField]
    private bool damageFallOff;
    [Tooltip("Damage through enemies")]
    [Range(0f, 1f)]
    [SerializeField]
    private float enemyPenetrationFactor;
    [Tooltip("Damage through walls")]
    [Range(0f, 1f)]
    [SerializeField]
    private float wallPenetrationFactor;
    private bool triggerHeld = false;
    private bool triggerPulled = false;

    [Header("Recoil")]
    [SerializeField] private Transform recoilPosition;
    [Range(0f, 1f)]
    [SerializeField] private float recoilAmount;
    [Range(0.1f, 1f)]
    [SerializeField] private float recoilRecoveryAmount;
    private float currentRecoil;


    [Header("Effects")]
    [SerializeField]
    private LayerMask hitLayerMask;
    [SerializeField]
    private Transform muzzleSocket;
    [SerializeField]
    private Projectile projectile;
    [SerializeField]
    private ParticleSystem muzzlePasticleSystem;


    private Transform cameraTransform;
    private int Damage => PlayerBuffsManager.Instance.IsBuffActive(Buffs.ExtraDamage10Percent) ? Mathf.RoundToInt((damage + (damage * (10f / 100f)))) : damage;


    void Start()
    {
        DebugUtility.HandleEmptyLayerMask(hitLayerMask, this, "Enemy/Floor/Walls");

        DebugUtility.HandleErrorIfNullGetComponent(muzzlePasticleSystem, this);
        DebugUtility.HandleErrorIfNullGetComponent(projectile, this);
        DebugUtility.HandleErrorIfNullGetComponent(muzzleSocket, this);

        cameraTransform = Camera.main?.transform;
        DebugUtility.HandleErrorIfNullGetComponent(cameraTransform, this);
    }

    private void LateUpdate()
    {
        triggerHeld = triggerPulled;
        triggerPulled = false;
    }

    private void Update()
    {
        AccumulateRecoil(-recoilRecoveryAmount * Time.deltaTime * 10);

        transform.localPosition = Vector3.Lerp(Vector3.zero, recoilPosition.localPosition, currentRecoil);
    }

    private void AccumulateRecoil(float delta)
    {
        currentRecoil = Mathf.Clamp01(currentRecoil + delta);
    }

    private Ray[] GetBulletRays()
    {
        Transform cameraTransform = Camera.main.transform;
        Ray[] rays = new Ray[shotCount];

        for (int i = 0; i < shotCount; i++)
        {
            rays[i] = new Ray(cameraTransform.position, GetShotDirection());
        }
        return rays;
    }

    private bool TraceRay(Ray ray)
    {
        bool successfullHit = false;
        float decayedDamage = Damage; // damage adjusted for penetration
        int calculatedDamage;
        HashSet<int> enemyInstanceIds; // don't hit the same enemy twice
        EnemyHitbox hitBox;

        float distance = 0f;

        RaycastHit[] hits = Physics.RaycastAll(ray, range, hitLayerMask);
        // For each enemy hit
        for (int i = 0; i < hits.Length; i++)
        {
            enemyInstanceIds = new HashSet<int>();
            calculatedDamage = Mathf.CeilToInt(decayedDamage);
            hitBox = hits[i].collider.gameObject.GetComponent<EnemyHitbox>();
            if (hitBox != null)
            {
                int enemyInstanceId = hitBox.Owner.gameObject.GetInstanceID();
                if (enemyInstanceIds.Contains(enemyInstanceId))
                {
                    // We already hit this enemy
                }
                else
                {
                    enemyInstanceIds.Add(enemyInstanceId);
                    successfullHit = true;
                    if (damageFallOff)
                    {
                        // Damage should fall of linearly with distance
                        float distanceToEnemy = Vector3.Distance(cameraTransform.position, hitBox.transform.position);
                        calculatedDamage = Mathf.CeilToInt(Mathf.Clamp01((range - distanceToEnemy) / range) * decayedDamage);

                        if (distanceToEnemy < distance)
                        {
                            Debug.LogError("Enemies Hit In Wrong Order");
                        }

                        distance = distanceToEnemy;
                    }

                    hitBox.TakeDamage(calculatedDamage);

                    decayedDamage *= enemyPenetrationFactor;

                }


            }
            else
            {
                decayedDamage *= wallPenetrationFactor;
            }

            // Exit Early if no point in tracing.
            if (hitBox == null || Mathf.FloorToInt(decayedDamage) == 0)
            {
                break;
            }
        }

        return successfullHit;

    }

    /// <summary>
    /// Firing logic
    /// </summary>
    private void Fire()
    {
        lastFireTime = Time.time;

        Color debugRayColor;
        bool successfullHit; //for debugging

        Ray[] rays = GetBulletRays();

        // For each shot fired
        for (int i = 0; i < rays.Length; i++)
        {
            Ray ray = rays[i];
            successfullHit = TraceRay(rays[i]);
            debugRayColor = successfullHit ? Color.green : Color.red;

            //Calculate the particle trajectories a little differently
            Instantiate(projectile, muzzleSocket.position + ray.direction, Quaternion.LookRotation(ray.direction));
            Debug.DrawLine(cameraTransform.position, cameraTransform.position + ray.direction * range, debugRayColor, 1f);

        }


        muzzlePasticleSystem.Play();
        AccumulateRecoil(recoilAmount);
    }

    /// <summary>
    /// Attempt the fire action. Firing may be hindered by the need to reload 
    /// or the rate of fire.
    /// </summary>
    private void TryFire()
    {
        triggerPulled = true;

        if (Time.time - lastFireTime > 10 / fireRate)
        {
            if (weaponFireType == FireType.SINGLE && !triggerHeld)
            {
                Fire();
            }
            else if (weaponFireType == FireType.AUTO)
            {
                Fire();
            }
        }

    }

    public override void Use(bool held)
    {
        TryFire();
    }

    /// <summary>
    /// Get normalized shot direction based on current
    /// </summary>
    private Vector3 GetShotDirection()
    {
        float spreadAngleRatio = spread;
        Vector3 spreadWorldDirection = Vector3.Slerp(cameraTransform.forward, UnityEngine.Random.insideUnitSphere,
            spreadAngleRatio).normalized;

        return spreadWorldDirection;
    }

}
