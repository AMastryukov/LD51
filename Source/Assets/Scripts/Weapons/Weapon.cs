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
    protected FireType weaponFireType = FireType.SINGLE;
    [SerializeField]
    protected int damage = 5;
    [SerializeField]
    [Range(1f, 20f)]
    protected float fireRate = 5;

    protected float lastFireTime = -Mathf.Infinity;
    [SerializeField]
    [Range(1, 10)]
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

    protected bool triggerHeld = false;
    protected bool triggerPulled = false;

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
    private float projectileLifeSpan = 0.1f;
    private Queue<Projectile> projectileQueue;
    [SerializeField] protected ParticleSystem muzzlePasticleSystem;

    protected AudioSource audioSource;


    private Transform cameraTransform;
    private int Damage => PlayerBuffsManager.Instance.IsBuffActive(Buffs.ExtraDamage10Percent) ? Mathf.RoundToInt((damage + (damage * (10f / 100f)))) : damage;


    void Start()
    {
        DebugUtility.HandleEmptyLayerMask(hitLayerMask, this, "Enemy/Floor/Walls");

        DebugUtility.HandleErrorIfNullGetComponent(muzzlePasticleSystem, this);
        //DebugUtility.HandleErrorIfNullGetComponent(projectile, this);
        DebugUtility.HandleErrorIfNullGetComponent(muzzleSocket, this);

        cameraTransform = Camera.main?.transform;
        DebugUtility.HandleErrorIfNullGetComponent(cameraTransform, this);

        audioSource = GetComponent<AudioSource>();
        DebugUtility.HandleErrorIfNullGetComponent(audioSource, this);

        projectileQueue = new Queue<Projectile>();

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
        transform.localRotation = Quaternion.Lerp(Quaternion.identity, recoilPosition.localRotation, currentRecoil);
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
        enemyInstanceIds = new HashSet<int>();
        
        RaycastHit[] hits = Physics.RaycastAll(ray, range, hitLayerMask);
        // For each enemy hit
        for (int i = 0; i < hits.Length; i++)
        {
            calculatedDamage = Mathf.CeilToInt(decayedDamage);
            hitBox = hits[i].collider.gameObject.GetComponent<EnemyHitbox>();
            if (hitBox != null)
            {
                int enemyInstanceId = hitBox.Owner.gameObject.GetInstanceID();
                if (enemyInstanceIds.Contains(enemyInstanceId))
                {
                    Debug.Log("Already hit");
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
            if (Mathf.FloorToInt(decayedDamage) == 0)
            {
                break;
            }
        }

        return successfullHit;

    }

    private void DequeuProjectile(Ray ray)
    {
        Vector3 pos = muzzleSocket.position + ray.direction;
        Quaternion rot = Quaternion.LookRotation(ray.direction);
        Projectile pj;

        if (projectileQueue.Count == 0 || projectileQueue.Peek().gameObject.activeSelf)
        {

            pj = Instantiate(projectile);

        }
        else
        {
            pj = projectileQueue.Dequeue();
        }

        pj.Initialize(projectileLifeSpan, pos, rot);
        projectileQueue.Enqueue(pj);



    }

    /// <summary>
    /// Firing logic
    /// </summary>
    protected virtual void Fire()
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
            DequeuProjectile(ray);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * range, debugRayColor, 1f);

        }

        audioSource.Play();
        muzzlePasticleSystem.Play();
        AccumulateRecoil(recoilAmount);
    }

    /// <summary>
    /// Attempt the fire action. Firing may be hindered by the need to reload 
    /// or the rate of fire.
    /// </summary>
    protected void TryFire()
    {
        triggerPulled = true;

        if (Time.time - lastFireTime > 1 / fireRate)
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

    public override bool Use(bool held)
    {
        TryFire();
        return false;
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
