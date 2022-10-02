using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBlueprint : Item
{
    [SerializeField] private float placementRange;
    [SerializeField] private LayerMask placementLayer;
    [SerializeField] private GameObject trapObj;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    private Camera mainCamera;
    private bool _isEquipped = false;
    private Mesh _objMesh;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        mainCamera=Camera.main;
        _objMesh = trapObj.GetComponent<MeshFilter>().mesh;
        _meshRenderer = trapObj.GetComponent<MeshRenderer>();
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
            trapObj.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 2f;//+new Vector3(0,_objMesh.bounds.extents.y,0);
            RaycastHit rayHit;
            
            Debug.DrawLine(trapObj.transform.position, trapObj.transform.position + Vector3.down * placementRange, Color.green, 1f);
            if (Physics.BoxCast(trapObj.transform.position,_objMesh.bounds.extents,Vector3.down,out rayHit,transform.rotation,placementRange,placementLayer))
            {
                if (rayHit.collider.gameObject.layer == 3)
                {
                    trapObj.transform.position = new Vector3(rayHit.point.x,rayHit.point.y,rayHit.point.z);
                    if (rayHit.collider.gameObject.transform != null)
                    {
                        _meshRenderer.material = validMaterial;
                    }
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        ConfirmBluePrintPlacement();
                    }
                }
                else
                {
                    trapObj.transform.position = new Vector3(rayHit.point.x,rayHit.point.y+_objMesh.bounds.extents.y,rayHit.point.z);
                    if (rayHit.collider.gameObject.transform != null)
                    {
                        _meshRenderer.material = invalidMaterial;
                    }
                }
            }
            else
            {
                //_meshRenderer.material = invalidMaterial;
            }
            trapObj.SetActive(true);
        }
        else
        {
            trapObj.SetActive(false);
        }
    }

    public override void Use()
    {
        //Do smth
    }

    private void Equip()
    {
        _isEquipped = !_isEquipped;
    }

    private void ConfirmBluePrintPlacement()
    {
        Debug.Log("Placement Confirmed");
    }
}
