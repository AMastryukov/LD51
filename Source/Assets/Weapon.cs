using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    [SerializeField]
    private float spread;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float range = 10f;
    [SerializeField]
    private AnimationCurve damageFallOff;
    [SerializeField]
    private LayerMask hitLayerMask;

    void Start()
    {
        DebugUtility.HandleEmptyLayerMask(hitLayerMask, this, "Enemy/Floor/Walls");
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
        Debug.Log("FIRE");

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out rayHit, range, hitLayerMask))
        {
            hitEnemy = rayHit.collider.gameObject.GetComponent<Enemy>();
        }


        if (hitEnemy != null)
        {
            hitEnemy.TakeDamage();
            Debug.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * range, Color.green, 1f);
        }
        else
        {
            Debug.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * range, Color.red, 1f);
        }

    }

    public override void Use()
    {
        TryFire();
    }

}
