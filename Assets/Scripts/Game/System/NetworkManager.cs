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
    public const byte MAX_PLAYERS = 4;

    public static NetworkManager Singleton { get; private set; }

    public UnityEvent OnConnectedEvent { get { return onConnected; } }
    public UnityEvent OnJoinRoomFailedEvent { get { return onJoinRoomFailed; } }

    [SerializeField] private UnityEvent onConnected;
    [SerializeField] private UnityEvent onJoinRoomFailed;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Singleton = this;
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

    [PunRPC]
    public void ForceStartGame()
    {
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;
        GameManager.Singleton.LobbyMenu.OnGameStart.Invoke();
    }

    [PunRPC]
    public void OnPlayerRematch(string username)
    {
        GameManager.Singleton.LobbyMenu.OnPlayerJoined(username);
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
        options.MaxPlayers = MAX_PLAYERS;
        PhotonNetwork.CreateRoom(GenerateRoomCode(), options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(LoadGameAndSpawn());
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player player)
    {
        GameManager.Singleton.LobbyMenu.OnPlayerJoined(player.NickName);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player player)
    {
        GameManager.Singleton.LobbyMenu.OnPlayerLeft(player.NickName);
    }

    private void SpawnLocalPlayer()
    {
        Transform spawnPoint = GameManager.PlayerSpawnSystem.GetSpawnPoint();
        Transform player = PhotonNetwork.Instantiate("Player", spawnPoint.position, spawnPoint.rotation, 0).transform;
        GameManager.SpectatorSystem.SetPlayer(player.GetComponent<Player>());
        Camera.main.GetComponent<CameraFollow>().FollowTarget(player);
    }

    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            onConnected.Invoke();
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
        
        Room room = PhotonNetwork.CurrentRoom;

        foreach(Photon.Realtime.Player player in room.Players.Values)
        {
            GameManager.Singleton.LobbyMenu.OnPlayerJoined(player.NickName);
        }

        GameManager.Singleton.CodeText.SetText(room.Name);
        GameManager.Singleton.LobbyMenu.OnGameStart.AddListener(SpawnLocalPlayer);
    }

    public static string GenerateRoomCode() //Generates a random and (hopefully) unique 5 digit room code.
    {
        long t = System.DateTime.Now.Ticks % 46656; //46656 = 36^3
        int rand = Random.Range(0, 1296); //1296 = 36^2
        string t36 = BaseConverter.ToBase(t, 36).PadLeft(3, '0').ToUpper();
        string rand36 = BaseConverter.ToBase(rand, 36).PadLeft(2, '0').ToUpper();
        return t36 + rand36;
    }
}
