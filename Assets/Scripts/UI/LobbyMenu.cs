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
    }

    private IEnumerator StartGame()
    {
        yield return gameStartWait;
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
