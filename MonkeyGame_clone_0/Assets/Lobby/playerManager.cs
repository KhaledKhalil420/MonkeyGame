using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class playerManager : NetworkBehaviour
{
    public NetworkObject playerPrefab;
    public Transform[] spawnPoint;

    Lobby lobby;

    bool spawnedPlayers = false;

    void Start()
    {
        spawnPlayerServerRpc(NetworkManager.LocalClientId);
    }

    IEnumerator spawnPlayers()
    {
        yield return new WaitForSeconds(2);
        //spawn a player prefab for each player in the joibed lobby
        for (int i = 0; i < lobbyManager.Instance.currentLobby.Players.Count; i++)
        {
            NetworkObject p = NetworkManager.Instantiate(playerPrefab, spawnPoint[i].position, Quaternion.identity);
            int playerId;
            int.TryParse(lobbyManager.Instance.currentLobby.Players[i].Data["playerId"].Value, out playerId);
            p.SpawnAsPlayerObject((ulong)playerId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void spawnPlayerServerRpc(ulong id)
    {
        int randomSpawnPoint = Random.Range(0, spawnPoint.Length);
        NetworkObject p = NetworkManager.Instantiate(playerPrefab, spawnPoint[randomSpawnPoint].position, Quaternion.identity);
        //int playerId;
        //int.TryParse(lobbyManager.Instance.currentLobby.Players[(int)NetworkManager.LocalClientId].Data["playerId"].Value, out playerId);
        p.SpawnAsPlayerObject((ulong)id);
    }
}
