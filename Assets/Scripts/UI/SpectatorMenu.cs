using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SpectatorMenu : HUDMenu
{
    public static SpectatorMenu Singleton { get; private set; }

    [SerializeField] private Text testText;

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

    public void SetTestText(string text)
    {
        testText.text = text;
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
