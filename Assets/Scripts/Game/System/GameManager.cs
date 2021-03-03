using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScoreSystem))]
public class GameManager : MonoBehaviour
{
    public static bool IsReady { get; private set; }
    public static ScoreSystem ScoreSystem { get { return scoreSystem; } }
    private static ScoreSystem scoreSystem;
    public static WaveSystem WaveSystem { get { return waveSystem; } }
    private static WaveSystem waveSystem;
    public static PlayerSpawnSystem PlayerSpawnSystem { get { return playerSpawnSystem; } }
    private static PlayerSpawnSystem playerSpawnSystem;
    public static SpectatorSystem SpectatorSystem { get { return spectatorSystem; } }
    private static SpectatorSystem spectatorSystem;

    public static GameManager Singleton { get { return singleton; } }
    private static GameManager singleton;

    public TMPro.TextMeshProUGUI CodeText { get { return codeText; } }
    [SerializeField] private TMPro.TextMeshProUGUI codeText;

    void Awake() 
    {
        //Enfore singleton
        if(singleton != null) 
        {
            Destroy(gameObject);
            return;
        }

        //Initialize singleton
        singleton = this;

        //Initialize systems
        scoreSystem = GetComponent<ScoreSystem>();
        waveSystem = GetComponent<WaveSystem>();
        playerSpawnSystem = GetComponent<PlayerSpawnSystem>();
        spectatorSystem = GetComponent<SpectatorSystem>();

        IsReady = true;
    }
}
