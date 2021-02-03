using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI textMeshProUGUI;

    void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        GameManager.ScoreSystem.AddScoreChangedListener(UpdateScore);
        UpdateScore(GameManager.ScoreSystem.Score);
    }

    public void UpdateScore(int score)
    {
        textMeshProUGUI.text = score.ToString();
    }
}
