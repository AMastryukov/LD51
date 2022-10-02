using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Tooltip("LifeTime of the projectile")]
    public float MaxLifeTime = 5f;

    [Tooltip("VFX prefab to spawn upon impact")]
    public GameObject ImpactVfx;

    [Header("Movement")]
    [Tooltip("Speed of the projectile")]
    public float Speed = 200f;



    void OnEnable()
    {
        Destroy(gameObject, MaxLifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
