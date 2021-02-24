using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public const string GAME_VERSION = "0.1";

    [SerializeField] private byte maxPlayers = 4;
    [SerializeField] private Transform[] playerSpawns;

    void Start()
    {
        Connect();
    }

    void OnApplicationQuit()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(null, options, TypedLobby.Default); //Passing null to the name field generates a GUID for name
    }

    public override void OnJoinedRoom()
    {
        SpawnLocalPlayer(0);
    }

    private void SpawnLocalPlayer(int spawnPoint)
    {
        Transform player = PhotonNetwork.Instantiate("Player", playerSpawns[spawnPoint].position, Quaternion.identity, 0).transform;
        Camera.main.GetComponent<CameraFollow>().FollowTarget(player);
    }

    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("Cannot connect when already connected!");
            return;
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = GAME_VERSION;
    }
}
