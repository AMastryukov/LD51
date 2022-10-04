using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPrompt : MonoBehaviour
{
    private void Awake()
    {
        PlayerInteractor.OnInteract += Display;
    }

    private void Display(bool display)
    {
        gameObject.SetActive(display);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
