using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPrompt : MonoBehaviour
{
    private void Awake()
    {
        PlayerInteractor.OnInteract += Display;
    }

    private void OnDestroy()
    {
        PlayerInteractor.OnInteract -= Display;
    }

    private void Display(bool display)
    {
        gameObject.SetActive(display);
    }
}
