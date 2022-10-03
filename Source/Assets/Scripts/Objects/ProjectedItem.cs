using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectedItem : Item
{
    [SerializeField] private GameObject prefabPreview;
    [SerializeField] private GameObject prefabToSpawn;
    private Transform cameraTransform;
    GameObject preview;

    [SerializeField]
    private LayerMask groundMask;

    bool previewing = false;

    public override bool Use(bool held)
    {
        if (!held && previewing)
        {
            Instantiate(prefabToSpawn, preview.transform.position, preview.transform.rotation);
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
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 5f, groundMask))
        {
            preview.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up)));
            previewing = true;
        }
        else
        {
            preview.transform.SetLocalPositionAndRotation(Vector3.forward, Quaternion.identity);
            previewing = false;
        }
    }
}
