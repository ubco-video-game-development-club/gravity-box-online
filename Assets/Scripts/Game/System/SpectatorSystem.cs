using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorSystem : MonoBehaviour
{
    [SerializeField] private CameraFollow camera;
    private Player localPlayer;

    public void SetPlayer(Player player)
    {
        if(localPlayer != null)
        {
            localPlayer.RemoveDeathListener(OnPlayerDie);
        }

        localPlayer = player;
        player.AddDeathListener(OnPlayerDie);
    }

    private void OnPlayerDie()
    {
        Player[] players = FindObjectsOfType<Player>();
        if(players.Length <= 1) return;

        Player player = players[Random.Range(0, players.Length)];
        while(player == localPlayer) player = players[Random.Range(0, players.Length)];
        SetPlayer(player);

        camera.FollowTarget(player.transform);
    }
}
