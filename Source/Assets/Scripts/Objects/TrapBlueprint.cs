using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBlueprint : Item
{
    [SerializeField] private float placementRange;
    [SerializeField] private LayerMask placementLayer;
    [SerializeField] private GameObject trapObj;
    private Camera mainCamera;
    private bool _isEquipped = false;
    private Mesh _objMesh;

    private void Awake()
    {
        mainCamera=Camera.main;
        _objMesh = trapObj.GetComponent<MeshFilter>().mesh;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Equip();
        }

        if (_isEquipped)
        {
            RaycastHit rayHit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out rayHit, placementRange, placementLayer))
            {
                if (rayHit.collider.gameObject.transform != null)
                {
                    trapObj.transform.position = new Vector3(rayHit.point.x,rayHit.point.y+_objMesh.bounds.extents.y,rayHit.point.z);
                    trapObj.SetActive(true);
                }
            }
            else
            {
                trapObj.SetActive(false);
            }
        }
    }

    public override void Use()
    {
        //Do smth
    }

    private void Equip()
    {
        _isEquipped = true;
    }
}
