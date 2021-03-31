using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Singleton { get; private set; }

    public bool IsPaused 
    {
        get 
        {
            return m_isPaused;
        }

        private set
        {
            if(GameManager.IsMenuActive && value) return;
            m_isPaused = value;
            panel.SetActive(value);
            GameManager.IsMenuActive = value;
        }
    }

   [SerializeField] private GameObject panel;
   private bool m_isPaused = false;

   void Awake()
   {
       if(panel == null)
       {
           Debug.LogError("Panel cannot be null!");
           enabled = false;
           return;
       }

       if(Singleton != null)
       {
           Debug.LogError("Cannot have more than one pause menu.");
           Destroy(gameObject);
           return;
       }

       Singleton = this;
   }

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
        }
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Play()
    {
        IsPaused = false;
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
