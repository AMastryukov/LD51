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
    private float spread;
    [SerializeField]
    private int damage = 5;
    [SerializeField]
    [Tooltip("Damage will be 0 at the end of the range")]
    private float range = 10f;
    [Tooltip("Damage will fall off linearly over the range")]
    [SerializeField]
    private bool damageFallOff;

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

    private int Damage => PlayerBuffsManager.Instance.IsBuffActive(Buffs.ExtraDamage10Percent) ? Mathf.RoundToInt((damage + (damage * (10f / 100f)))) : damage;

    void Start()
    {
        DebugUtility.HandleEmptyLayerMask(hitLayerMask, this, "Enemy/Floor/Walls");

        DebugUtility.HandleErrorIfNullGetComponent(muzzlePasticleSystem, this);
        DebugUtility.HandleErrorIfNullGetComponent(projectile, this);
        DebugUtility.HandleErrorIfNullGetComponent(muzzleSocket, this);
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

    /// <summary>
    /// Attempt the fire action. Firing may be hindered by the need to reload 
    /// or the rate of fire.
    /// </summary>
    private void TryFire()
    {
        RaycastHit rayHit;
        Enemy hitEnemy = null;
        Transform cameraTransform = Camera.main.transform;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out rayHit, range, hitLayerMask))
        {
            hitEnemy = rayHit.collider.gameObject.GetComponent<Enemy>();
        }


        if (hitEnemy != null)
        {
            hitEnemy.TakeDamage(Damage);
            Debug.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * range, Color.green, 1f);
        }
        else
        {
            Debug.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * range, Color.red, 1f);
        }

        // TODO: this could probably be cleaned up
        Instantiate(projectile, muzzleSocket.position, transform.rotation);
        muzzlePasticleSystem.Play();
        AccumulateRecoil(recoilAmount);


    }

    public override void Use()
    {
        TryFire();
    }

    /// <summary>
    /// Get shot direction based on current
    /// </summary>
    private void GetShotDirection()
    {

    }

    private void ApplyRecoil()
    {

    }

}
