using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns the player in a random room (doesn't create the player, just positions them on Start)
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawns;

    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        _player.transform.position = spawns[Random.Range(0, spawns.Length)].position;
    }
}
