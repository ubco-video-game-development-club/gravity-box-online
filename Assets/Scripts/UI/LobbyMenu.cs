using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyMenu : HUDMenu
{
    public UnityEvent OnGameStart { get { return onGameStart; } }

    [SerializeField] private TextMeshProUGUI waitingText;
    [SerializeField] private TextMeshProUGUI waitingDots;
    [SerializeField] private GameObject forceStartButton;
    [SerializeField] private TextMeshProUGUI[] playerNames;
    [SerializeField] private float dotTime = 0.5f;
    [SerializeField] private UnityEvent onGameStart;
    private int playerCount = 0;
    private YieldInstruction animationWait, gameStartWait;

    protected override void Awake()
    {
        animationWait = new WaitForSeconds(dotTime);
        gameStartWait = new WaitForSeconds(1.0f);
        StartCoroutine(Animate());
        base.Awake();
        OnOpen();
    }

    public void OnOpen()
    {
        playerCount = 0;
        GameManager.IsMenuActive = true;
        forceStartButton.SetActive(PhotonNetwork.IsMasterClient);
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = true;
    }

    public void OnPlayerJoined(string name)
    {
        if (playerCount >= playerNames.Length)
        {
            Debug.LogError("Too many players for lobby menu!");
            return;
        }

        playerNames[playerCount++].SetText(name);
        waitingText.SetText($"Waiting ({playerCount}/{NetworkManager.MAX_PLAYERS})");

        if(playerCount == NetworkManager.MAX_PLAYERS) StartCoroutine(StartGame());
    }

    public void OnPlayerLeft(string name)
    {
        playerCount = 0;
        Room room = PhotonNetwork.CurrentRoom;
        foreach(Photon.Realtime.Player player in room.Players.Values)
        {
            OnPlayerJoined(player.NickName);
        }

        forceStartButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void OnStartNowClicked() 
    {
        NetworkManager netManager = NetworkManager.Singleton;
        netManager.photonView.RPC("ForceStartGame", RpcTarget.All);
        GameManager.IsMenuActive = false;
    }

    public void OnLeaveClicked() => StartCoroutine(LeaveGame());

    private IEnumerator LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        yield return new WaitUntil(() => !PhotonNetwork.InRoom);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private IEnumerator StartGame()
    {
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;
        yield return gameStartWait;
        GameManager.IsMenuActive = false;
        onGameStart.Invoke();
    }

    private IEnumerator Animate()
    {
        int dots = 0;

        while (enabled)
        {
            string dotStr = ".";
            for (int i = 0; i < dots % 3; i++)
            {
                dotStr += ".";
            }

            dots++;

            waitingDots.text = dotStr;
            for (int i = playerCount; i < playerNames.Length; i++)
            {
                playerNames[i].SetText(dotStr);
            }

            yield return animationWait;
        }
    }
}
