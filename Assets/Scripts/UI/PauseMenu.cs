using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            m_isPaused = value;
            panel.SetActive(value);
            Time.timeScale = value ? 0.0f : 1.0f; //Set time scale to zero if the game is paused; otherwise 1
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
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
