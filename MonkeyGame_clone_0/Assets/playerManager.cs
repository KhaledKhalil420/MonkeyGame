using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class playerManager : NetworkBehaviour
{
    public NetworkObject playerPrefab;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        //spawn spawn a player prefab for each player in the joibed lobby
        if(!NetworkManager.Singleton.IsHost) return;
        foreach (Player player in lobbyManager.Instance.GetJoinedLobby().Players)
        {
            NetworkObject p = NetworkManager.Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            int playerId;
            int.TryParse(player.Data["playerId"].Value, out playerId);
            p.SpawnAsPlayerObject((ulong)playerId);
        }
    }
}
