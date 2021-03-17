using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerSpawnSystem : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public Transform GetSpawnPoint()
    {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        int idx = Array.IndexOf(players, PhotonNetwork.LocalPlayer);
        return spawnPoints[idx];
    }
}
