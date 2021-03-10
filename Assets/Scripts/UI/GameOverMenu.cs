using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameOverMenu : MonoBehaviour
{
    public static GameOverMenu Singleton { get; private set; }

    [SerializeField] private 

    void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
    }

    public void EndGame()
    {
        Player winner = FindObjectOfType<Player>();
        bool isWinner = winner.GetComponent<PhotonView>().IsMine;
    }

    public void OnPlayAgainButtonClicked()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OnQuitButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
}
