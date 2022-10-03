using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the enemy ragdoll and gibs
/// </summary>
public class EnemyCorpse : MonoBehaviour
{
    [SerializeField] private GameObject ragdoll;
    [SerializeField] private Rigidbody[] ragdollRigibodies;

    [Header("Bones")]
    [SerializeField] private Transform headBone;
    [SerializeField] private Transform leftArmBone;
    [SerializeField] private Transform rightArmBone;
    [SerializeField] private Transform leftLegBone;
    [SerializeField] private Transform rightLegBone;

    [Header("Gibs")]
    [SerializeField] private Rigidbody[] headGibs;
    [SerializeField] private Rigidbody[] bodyGibs;
    [SerializeField] private Rigidbody[] leftArmGibs;
    [SerializeField] private Rigidbody[] rightArmGibs;
    [SerializeField] private Rigidbody[] leftLegGibs;
    [SerializeField] private Rigidbody[] rightLegGibs;

    private List<Rigidbody> _allGibs = new List<Rigidbody>();
    private Coroutine _cleanup;


    private void Awake()
    {
        // Disable ragdoll by default
        foreach (var bodyPart in ragdollRigibodies)
        {
            bodyPart.isKinematic = true;
        }

        _allGibs.AddRange(headGibs);
        _allGibs.AddRange(bodyGibs);
        _allGibs.AddRange(leftArmGibs);
        _allGibs.AddRange(rightArmGibs);
        _allGibs.AddRange(leftLegGibs);
        _allGibs.AddRange(rightLegGibs);
    }

    public void EnableRagdoll()
    {
        foreach (var bodyPart in ragdollRigibodies)
        {
            bodyPart.isKinematic = false;
            bodyPart.useGravity = true;
        }

        if (_cleanup == null)
        {
            _cleanup = StartCoroutine(Cleanup());
        }
    }

    public void ExplodeExtremities()
    {
        ExplodeBodyPart(Enemy.BodyPart.LeftArm);
        ExplodeBodyPart(Enemy.BodyPart.RightArm);
        ExplodeBodyPart(Enemy.BodyPart.LeftLeg);
        ExplodeBodyPart(Enemy.BodyPart.RightLeg);

        if (_cleanup == null)
        {
            _cleanup = StartCoroutine(Cleanup());
        }
    }

    public void ExplodeBodyPart(Enemy.BodyPart bodyPart)
    {
        Transform boneToDisable = null;
        Rigidbody[] gibsToEnable = new Rigidbody[0];

        switch (bodyPart)
        {
            case Enemy.BodyPart.Head:
                boneToDisable = headBone;
                gibsToEnable = headGibs;
                break;

            case Enemy.BodyPart.Body:
                gibsToEnable = bodyGibs;
                break;

            case Enemy.BodyPart.LeftArm:
                boneToDisable = leftArmBone;
                gibsToEnable = leftArmGibs;
                break;

            case Enemy.BodyPart.RightArm:
                boneToDisable = rightArmBone;
                gibsToEnable = rightArmGibs;
                break;

            case Enemy.BodyPart.LeftLeg:
                boneToDisable = leftLegBone;
                gibsToEnable = leftLegGibs;
                break;

            case Enemy.BodyPart.RightLeg:
                boneToDisable = rightLegBone;
                gibsToEnable = rightLegGibs;
                break;
        }

        // Set scale of associated bone to 0
        if (boneToDisable != null)
        {
            boneToDisable.transform.localScale = Vector3.zero;
        }

        // Enable associated gibs & set them to non-kinematic to have gravity
        foreach (var gib in gibsToEnable)
        {
            gib.gameObject.SetActive(true);

            gib.isKinematic = false;
            gib.useGravity = true;

            gib.AddExplosionForce(10f, gib.transform.position - Vector3.down, 2f, 2f, ForceMode.Impulse);
        }

        EnableRagdoll();

        if (_cleanup == null)
        {
            _cleanup = StartCoroutine(Cleanup());
        }
    }

    public void ExplodeCompletely()
    {
        // Disable the entire ragdoll, we don't need it
        ragdoll.SetActive(false);

        // Enable all gibs
        foreach (var gib in _allGibs)
        {
            gib.gameObject.SetActive(true);

            gib.isKinematic = false;
            gib.useGravity = true;

            gib.AddExplosionForce(20f, gib.transform.position - Vector3.down, 2f, 4f, ForceMode.Impulse);
        }

        if (_cleanup == null)
        {
            _cleanup = StartCoroutine(Cleanup());
        }
    }

    private IEnumerator Cleanup()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
