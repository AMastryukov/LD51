using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoomTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.TagConstants.EnemyTag))
        {
            other.gameObject.GetComponent<EnemyAi>().SetRoomState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameConstants.TagConstants.EnemyTag))
        {
            other.gameObject.GetComponent<EnemyAi>().SetRoomState(false);
        }
    }
}
