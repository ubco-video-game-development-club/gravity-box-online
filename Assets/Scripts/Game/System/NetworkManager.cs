using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public const string GAME_VERSION = "0.1";

    public UnityEvent OnConnectedEvent { get { return onConnected; } }
    public UnityEvent OnJoinRoomFailedEvent { get { return onJoinRoomFailed; } }

    [SerializeField] private byte maxPlayers = 4;
    [SerializeField] private UnityEvent onConnected;
    [SerializeField] private UnityEvent onJoinRoomFailed;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

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
        //PhotonNetwork.JoinRandomRoom();
        onConnected.Invoke();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Failed to join room: {message}::{returnCode}");
        onJoinRoomFailed.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(null, options, TypedLobby.Default); //Passing null to the name field generates a GUID for name
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(LoadGameAndSpawn());
    }

    private void SpawnLocalPlayer()
    {
        Transform spawnPoint = GameManager.PlayerSpawnSystem.GetSpawnPoint();
        Transform player = PhotonNetwork.Instantiate("Player", spawnPoint.position, spawnPoint.rotation, 0).transform;
        Camera.main.GetComponent<CameraFollow>().FollowTarget(player);
    }

    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("Cannot connect when already connected!");
            return;
        }

        PhotonNetwork.AutomaticallySyncScene = false; //We don't want to do this because of main menus and stuff
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = GAME_VERSION;
    }

    private IEnumerator LoadGameAndSpawn()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("Game");
        yield return new WaitUntil(() => load.isDone);
        yield return new WaitUntil(() => GameManager.IsReady);
        SpawnLocalPlayer(); //TODO: Lobby. Don't spawn immediately.
    }
}
