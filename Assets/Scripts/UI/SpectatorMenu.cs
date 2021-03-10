using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SpectatorMenu : MonoBehaviour
{
    public static SpectatorMenu Singleton { get; private set; }

    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1 : 0;
        canvasGroup.blocksRaycasts = visible;
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
