using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorSystem : MonoBehaviour
{
    [SerializeField] private new CameraFollow camera;
    private Player localPlayer;

    private bool isGameOver = false;

    public void SetPlayer(Player player)
    {
        if (localPlayer != null)
        {
            localPlayer.RemoveDeathListener(OnPlayerDie);
        }

        localPlayer = player;
        player.AddDeathListener(OnPlayerDie);
    }

    private void OnPlayerDie()
    {
        SpectatorMenu.Singleton.SetVisible(true);

        Player[] players = FindObjectsOfType<Player>();
        Player player = players[Random.Range(0, players.Length)];
        while (player == localPlayer) player = players[Random.Range(0, players.Length)];
        SetPlayer(player);

        camera.FollowTarget(player.transform);
    }
}
