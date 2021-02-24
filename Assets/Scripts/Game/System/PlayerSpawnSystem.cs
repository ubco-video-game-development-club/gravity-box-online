using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnSystem : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public Transform GetSpawnPoint()
    {
        return spawnPoints[0]; //TODO: Round-robin spawning and such.
    }
}
