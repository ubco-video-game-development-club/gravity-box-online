﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class GameOverMenu : HUDMenu
{
    public static GameOverMenu Singleton { get; private set; }

    [SerializeField] private TextMeshProUGUI headerText;

    protected override void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;

        base.Awake();
    }

    public void EndGame()
    {
        Player winner = FindObjectOfType<Player>();
        bool isWinner = winner.GetComponent<PhotonView>().IsMine;
        headerText.text = isWinner ? "YOU WIN" : "YOU LOSE";
        SpectatorMenu.Singleton.SetVisible(false);
        SetVisible(true);
    }

    public void Rematch()
    {
        SetVisible(false);
        GameManager.Singleton.LobbyMenu.SetVisible(true);
    }

    public void Quit()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(LeaveGame());
    }

    private IEnumerator LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        yield return new WaitUntil(() => !PhotonNetwork.InRoom);
        SceneManager.LoadScene(0);
    }
}
