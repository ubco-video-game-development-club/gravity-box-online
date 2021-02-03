using UnityEngine;
using UnityEngine.Events;

public class ScoreSystem : MonoBehaviour
{
    private const string HIGH_SCORE_PREF = "player.highscore";

    [System.Serializable] public class OnScoreChangedEvent : UnityEvent<int> { }

    public int Score { get { return score; } }
    private int score;
    private int highScore;
    private bool achievedHighScore = false;

    [SerializeField] private OnScoreChangedEvent onScoreChanged = new OnScoreChangedEvent();
    [SerializeField] private UnityEvent onNewHighscore = new UnityEvent();

    void Awake()
    {
        highScore = PlayerPrefs.GetInt(GetHighScorePref(Leaderboard.username));
    }

    public void AddScore(int amount) 
    {
        //This assumes we don't ever want to take away player score.
        //If we do, just change this accordingly.
        if(amount < 0) 
        {
            Debug.LogError("Cannot add negative score.");
            return;
        }

        score += amount;

        if(score > highScore)
        {
            if (!achievedHighScore) onNewHighscore.Invoke();
            
            achievedHighScore = true;
            PlayerPrefs.SetInt(GetHighScorePref(Leaderboard.username), score);
            highScore = score;
        }

        onScoreChanged.Invoke(score);
    }

    public void SaveHighscoreToLeaderboard()
    {
        StartCoroutine(Leaderboard.SetUserScore(Leaderboard.username, score));
    }

    public void AddScoreChangedListener(UnityAction<int> call) 
    {
        onScoreChanged.AddListener(call);
    }

    public void RemoveScoreChangedListener(UnityAction<int> call) 
    {
        onScoreChanged.RemoveListener(call);
    }

    public static string GetHighScorePref(string username)
    {
        return $"{HIGH_SCORE_PREF}.{username}";
    }
}
