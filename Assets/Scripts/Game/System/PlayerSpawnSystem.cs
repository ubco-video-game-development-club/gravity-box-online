using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawnSystem : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public Transform GetSpawnPoint()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        return spawnPoints[playerCount - 1];
    }
}
