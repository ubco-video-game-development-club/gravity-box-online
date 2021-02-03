using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class Leaderboard : MonoBehaviour
{
    [System.Serializable]
    private struct User
    {
        public string username;
        public int score;
		public bool owned;
    }

    [System.Serializable]
    private struct UserArray
    {
        public User[] array;
    }

    [System.Serializable]
    private struct PostInfo
    {
        public string key;
        public string username;
        public int score;
    }

	[System.Serializable]
	private struct RankInfo
	{
		public int rank;
		public string username;
		public int score;
	}

    private const string UID_PREF = "user.uid";
    private const string USER_AGENT = "Gravity Box Client";
    private const string API_END_POINT = "https://pear-periwinkle-cilantro.glitch.me";
    private const string API_KEY = "test";
    private const int NUM_ENTRIES = 10;

    public static string username = "Guest";

    [SerializeField] private LeaderboardEntry entryPrefab;
    [SerializeField] private LeaderboardEntry personalEntry;

    private LeaderboardEntry[] entries;

    void Awake()
    {
        entries = new LeaderboardEntry[NUM_ENTRIES];
        for (int i = 0; i < NUM_ENTRIES; i++)
        {
            entries[i] = Instantiate(entryPrefab, transform);
            entries[i].Initialize(i);
        }
    }

    void OnEnable()
    {
        StartCoroutine(GetLeaderboard());
    }

    private IEnumerator GetLeaderboard()
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{API_END_POINT}/top/{NUM_ENTRIES}?{GetOrCreateUID()}"))
        {
            request.SetRequestHeader("User-Agent", USER_AGENT);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                json = "{ \"array\": " + json + "}";
                UserArray topScores = JsonUtility.FromJson<UserArray>(json);
                DisplayScores(topScores.array);
            }
        }

        using(UnityWebRequest request = UnityWebRequest.Get($"{API_END_POINT}/rank/{GetOrCreateUID()}"))
        {
            request.SetRequestHeader("User-Agent", USER_AGENT);

            yield return request.SendWebRequest();

            if(request.isHttpError || request.isNetworkError)
            {
                Debug.LogError(request.error);
            } else
            {
                string response = request.downloadHandler.text;
				RankInfo rankInfo = JsonUtility.FromJson<RankInfo>(response);
                DisplayRank(rankInfo);
            }
        }
    }

    private void DisplayScores(User[] topScores)
    {
        for (int i = 0; i < NUM_ENTRIES; i++)
        {
            entries[i].DisplayScore(i + 1, topScores[i].username, topScores[i].score, topScores[i].owned);
        }
    }

    private void DisplayRank(RankInfo rankInfo)
    {
        if (rankInfo.rank < 0)
        {
            personalEntry.gameObject.SetActive(false); //This works because the scene will be reloaded if you get a high score
        }
        else
        {
            personalEntry.DisplayScore(rankInfo.rank + 1, rankInfo.username, rankInfo.score, true);
        }
    }

    public static IEnumerator SetUserScore(string username, int score)
    {
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("Empty username passed to SetUserScore.");
            yield break;
        }

        PostInfo post = new PostInfo() 
        {
            key = API_KEY,
            username = username,
            score = score
        };

        string json = JsonUtility.ToJson(post);

        using (UnityWebRequest request = new UnityWebRequest())
        {
            request.url = $"{API_END_POINT}/score/{GetOrCreateUID()}";
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;

            request.SetRequestHeader("User-Agent", USER_AGENT);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }

    public static string GetOrCreateUID()
    {
        string uid = PlayerPrefs.GetString(UID_PREF);
        if(uid == "")
        {
            uid = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(UID_PREF, uid);
        }

        return uid;
    }
}
