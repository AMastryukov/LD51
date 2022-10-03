using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewItem : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> meshes;
    // Start is called before the first frame update
    private void OnEnable()
    {
        ProjectedItem.OnMaterialChange += ChangeMaterial;
    }

    private void OnDisable()
    {
        ProjectedItem.OnMaterialChange -= ChangeMaterial;
    }

    private void ChangeMaterial(Material material)
    {
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material = material;
        }
    }
}
