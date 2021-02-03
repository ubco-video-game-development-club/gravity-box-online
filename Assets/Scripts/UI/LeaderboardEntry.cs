using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    public TMPro.TextMeshProUGUI RankText { get { return rankText; } }

    [SerializeField] private TMPro.TextMeshProUGUI rankText;
    [SerializeField] private TMPro.TextMeshProUGUI usernameText;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(int index)
    {
        rectTransform.anchoredPosition = new Vector2(0, index * -rectTransform.sizeDelta.y);
        rankText.text = (index + 1) + ".";
        usernameText.text = "-";
        scoreText.text = "-";
    }

    public void DisplayScore(int rank, string username, int score, bool owned)
    {
		if(owned)
		{
			usernameText.text = $"<b>{username}</b>";
			rankText.text = $"<b>{rank}.</b>";
        	scoreText.text = $"<b>{score}</b>";
		} else
		{
			usernameText.text = username;
			rankText.text = rank.ToString() + ".";
        	scoreText.text = score.ToString();
		}

    }
}
