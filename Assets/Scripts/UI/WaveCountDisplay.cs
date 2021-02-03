using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveCountDisplay : MonoBehaviour
{
    private TextMeshProUGUI textMeshProUGUI;

    void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        GameManager.WaveSystem.AddWaveCountChangedListener(UpdateWaveCount);
        UpdateWaveCount(GameManager.WaveSystem.WaveCount);
    }

    public void UpdateWaveCount(int waveCount)
    {
        textMeshProUGUI.text = "Wave " + waveCount.ToString();
    }
}
