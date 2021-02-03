using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class WaveSystem : MonoBehaviour
{
    [System.Serializable] public class OnTimerChangedEvent : UnityEvent<int> { }
    [System.Serializable] public class OnWaveCountChangedEvent : UnityEvent<int> { }

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private float baseSpawnInterval = 2f;
    [SerializeField] private float spawnIntervalDecrement = 0.05f;
    [SerializeField] private float baseSpawnCount = 1.5f;
    [SerializeField] private float spawnCountIncrement = 0.5f;
    [SerializeField] private int maxWaveDuration = 30;
    [SerializeField] private int startDelay = 3;
    [SerializeField] private int waveCutoffDuration = 3;
    [SerializeField] private OnTimerChangedEvent onTimerChanged = new OnTimerChangedEvent();
    [SerializeField] private OnWaveCountChangedEvent onWaveCountChanged = new OnWaveCountChangedEvent();
    private AudioSource audioSource;

    public int WaveTimer
    {
        get { return _waveTimer; }
        set
        {
            _waveTimer = value;
            if (_waveTimer <= 0)
            {
                StartCoroutine(SpawnWave());
                _waveTimer = maxWaveDuration;
            }

            onTimerChanged.Invoke(_waveTimer);

            if (tickCoroutine != null)
            {
                StopCoroutine(tickCoroutine);
            }
            tickCoroutine = StartCoroutine(TickWaveTimer());
        }
    }
    private int _waveTimer;

    public int WaveCount
    {
        get { return _waveCount; }
        set
        {
            _waveCount = value;
            onWaveCountChanged.Invoke(_waveCount);
        }
    }
    private int _waveCount;

    private float spawnInterval = 0;
    private int spawnCount = 0;
    private float rawSpawnCount = 0;
    private int enemiesRemaining = 0;

    private YieldInstruction spawnInstruction;
    private YieldInstruction tickInstruction;
    private Coroutine tickCoroutine;

    void Awake()
    {
        spawnInterval = baseSpawnInterval;
        rawSpawnCount = baseSpawnCount;
        spawnInstruction = new WaitForSeconds(spawnInterval);
        spawnCount = Mathf.FloorToInt(rawSpawnCount);
        tickInstruction = new WaitForSeconds(1);

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        WaveCount = 0;
        WaveTimer = startDelay;
    }

    public void RemoveEnemy()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0 && WaveTimer > waveCutoffDuration)
        {
            WaveTimer = waveCutoffDuration;
        }
    }

    private IEnumerator TickWaveTimer()
    {
        //If there's 3 seconds or less on the clock,
        if(WaveTimer <= 3)
        {
            //Play the countdown sound
            audioSource.Play();
        }

        yield return new WaitForSeconds(1);
        WaveTimer--;
    }

    private IEnumerator SpawnWave()
    {
        WaveCount++;
        enemiesRemaining += 4 * spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                enemy.AddDeathListener(RemoveEnemy);
            }

            yield return spawnInstruction;
        }

        UpdateWave();
    }

    private void UpdateWave()
    {
        // Increase difficulty
        spawnInterval = Mathf.Clamp(spawnInterval - spawnIntervalDecrement, 0.1f, baseSpawnInterval);
        rawSpawnCount += spawnCountIncrement;

        // Update wave data
        spawnInstruction = new WaitForSeconds(spawnInterval);
        spawnCount = Mathf.FloorToInt(rawSpawnCount);
    }

    public void AddTimerChangedListener(UnityAction<int> call) 
    {
        onTimerChanged.AddListener(call);
    }

    public void RemoveTimerChangedListener(UnityAction<int> call) 
    {
        onTimerChanged.RemoveListener(call);
    }

    public void AddWaveCountChangedListener(UnityAction<int> call) 
    {
        onWaveCountChanged.AddListener(call);
    }

    public void RemoveWWaveCountChangedListener(UnityAction<int> call) 
    {
        onWaveCountChanged.RemoveListener(call);
    }
}
