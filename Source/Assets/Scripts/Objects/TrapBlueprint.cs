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
    private bool _isEquipped = false;
    private Mesh _objMesh;
    private MeshRenderer _meshRenderer;
    private bool _onValidGround = false;

    private void Awake()
    {
        _objMesh = trapObj.GetComponent<MeshFilter>().mesh;
        _meshRenderer = trapObj.GetComponent<MeshRenderer>();

    }
    private void OnEnable()
    {
        GameManager.OnTenSecondsPassed += ExplicitUnEquip;
    }

    private void OnDisable()
    {
        GameManager.OnTenSecondsPassed -= ExplicitUnEquip;
    }
    
    void Update()
    {
        //Temporarily managing input here, should be hooked up to input handler
        if (Input.GetKeyDown(KeyCode.X))
        {
            Use();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ConfirmBluePrintPlacement();
        }

        if (_isEquipped)
        {
            RaycastHit rayHit;
            Debug.DrawLine(trapObj.transform.position+Vector3.up*placementRange/2, trapObj.transform.position + Vector3.down * placementRange, Color.green, 1f);
            if (Physics.BoxCast(trapObj.transform.position+Vector3.up*placementRange/2,_objMesh.bounds.extents,Vector3.down,out rayHit,transform.rotation,placementRange,placementLayer))
            {
                if (rayHit.collider.gameObject.layer == 3)
                {
                    _onValidGround = true;
                    _meshRenderer.material = validMaterial;
                    
                }
                else
                {
                    _onValidGround = false;
                    _meshRenderer.material = invalidMaterial;
                }
            }
            trapObj.SetActive(true);
        }
        else
        {
            _onValidGround = false;
            trapObj.SetActive(false);
        }
    }

    public override void Use()
    {
        _isEquipped = !_isEquipped;
    }

    private void ExplicitUnEquip()
    {
        _isEquipped = false;
    }

    private void ConfirmBluePrintPlacement()
    {
        if (!_isEquipped) return;
        if (_onValidGround)
        {
            Debug.Log("Placement Confirmed");
            ExplicitUnEquip();
        }
    }
}
