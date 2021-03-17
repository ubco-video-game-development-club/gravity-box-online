using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SpectatorMenu : HUDMenu
{
    public static SpectatorMenu Singleton { get; private set; }

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

    public void OnQuitButtonClicked()
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
