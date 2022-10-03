using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectedItem : Item
{
    [SerializeField] private GameObject prefabPreview;
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private float placementY = 0.35f;
    public static Action<Material> OnMaterialChange;
    private Transform cameraTransform;
    GameObject preview;
    private BoxCollider _collider;

    [SerializeField]
    private LayerMask groundMask;

    bool previewing = false;



    public override bool Use(bool held)
    {
        if (!held && previewing)
        {
            Vector3 newTrapPosition = preview.transform.position;
            newTrapPosition = new Vector3(newTrapPosition.x, placementY, newTrapPosition.z); // hack; the house is only 1 floor and we have less than 2 hours left. It works!
            Instantiate(prefabToSpawn, newTrapPosition, preview.transform.rotation);
            previewing = false;
            gameObject.SetActive(false);
            return true;
        }
        return false;

    }

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        DebugUtility.HandleErrorIfNullGetComponent(cameraTransform, this);

        DebugUtility.HandleErrorIfNullGetComponent(prefabPreview, this);
        DebugUtility.HandleErrorIfNullGetComponent(prefabToSpawn, this);


        DebugUtility.HandleEmptyLayerMask(groundMask, this, "Ground");
        preview = Instantiate(prefabPreview, transform);
        _collider = preview.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        RaycastHit hit2;
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit2, 6f, groundMask);
        if (Physics.BoxCast(cameraTransform.position, _collider.size / 2, cameraTransform.forward, out hit, transform.rotation, 3f, groundMask))
        {
            if (hit.collider.gameObject.layer != 3)
            {
                OnMaterialChange?.Invoke(invalidMaterial);
                preview.transform.SetLocalPositionAndRotation(Vector3.forward, Quaternion.identity);
                previewing = false;
                return;
            }
            //Debug.DrawLine(cameraTransform.position, cameraTransform.forward*5f , Color.green, 1f);

            OnMaterialChange?.Invoke(validMaterial);
            preview.transform.SetPositionAndRotation(new Vector3(hit2.point.x, hit.point.y, hit2.point.z), Quaternion.LookRotation(Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up)));
            previewing = true;
        }
        else
        {
            OnMaterialChange?.Invoke(invalidMaterial);
            previewing = false;
            preview.transform.SetLocalPositionAndRotation(Vector3.forward, Quaternion.identity);
        }
    }
}
