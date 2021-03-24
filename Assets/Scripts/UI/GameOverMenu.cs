using System.Collections;
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
        GameManager.IsMenuActive = true;
    }

    public void Rematch()
    {
        SetVisible(false);
        GameManager.IsMenuActive = false;
        GameManager.Singleton.LobbyMenu.SetVisible(true);
        GameManager.Singleton.LobbyMenu.OnOpen();
    }

    public void Quit()
    {
        StartCoroutine(LeaveGame());
    }

    private IEnumerator LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        yield return new WaitUntil(() => !PhotonNetwork.InRoom);
        SceneManager.LoadScene(0);
    }
}
