using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float MaxLifeTime = 0.1f;

    [Tooltip("VFX prefab to spawn upon impact")]
    public GameObject ImpactVfx;

    [Header("Movement")]
    [Tooltip("Speed of the projectile")]
    public float Speed = 200f;

    private float startTime;



    void OnEnable()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime > MaxLifeTime)
            Deactivate();
        else
            transform.position += transform.forward * Speed * Time.deltaTime;

    }

    /// <summary>
    /// Call when Dequeue
    /// </summary>
    /// <param name="maxLifeTime"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    public void Initialize(float maxLifeTime, Vector3 pos, Quaternion rot)
    {
        MaxLifeTime = maxLifeTime;
        transform.SetPositionAndRotation(pos, rot);
        gameObject.SetActive(true);
    }


    public void Deactivate()
    {
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        Deactivate();
    }
}
