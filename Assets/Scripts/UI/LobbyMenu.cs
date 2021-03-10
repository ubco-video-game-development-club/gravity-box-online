using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waitingText;
    [SerializeField] private TextMeshProUGUI waitingDots;
    [SerializeField] private TextMeshProUGUI[] playerNames;
    [SerializeField] private float dotTime = 0.5f;
    private int playerCount = 0;
    private YieldInstruction animationWait;

    void Awake()
    {
        animationWait = new WaitForSeconds(dotTime);
        StartCoroutine(Animate());
    }

    public void OnPlayerJoined(string name)
    {
        if(playerCount >= playerNames.Length)
        {
            Debug.LogError("Too many players for lobby menu!");
            return;
        }

        playerNames[playerCount++].SetText(name);
    }

    private IEnumerator Animate()
    {
        int dots = 0;

        while(enabled)
        {
            string dotStr = ".";
            for(int i = 0; i < dots % 3; i++)
            {
                dotStr += ".";
            }

            dots++;

            waitingDots.text = dotStr;
            for(int i = playerCount; i < playerNames.Length; i++)
            {
                playerNames[i].SetText(dotStr);
            }

            yield return animationWait;
        }
    }
}
