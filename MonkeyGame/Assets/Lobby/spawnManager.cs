using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class spawnManager : NetworkBehaviour
{
    public NetworkObject playerPrefab;
    public Transform[] spawnPoint;

    Lobby lobby;

    bool spawnedPlayers = false;

    public List<Teams> teams = new List<Teams>();

    public static spawnManager instance;

    void Start()
    {
        if(instance == null) instance = this;
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
        //spawn the player
        int randomSpawnPoint = Random.Range(0, spawnPoint.Length);
        NetworkObject p = NetworkManager.Instantiate(playerPrefab, spawnPoint[randomSpawnPoint].position, Quaternion.identity);
        p.SpawnAsPlayerObject((ulong)id);

        //add the player to a random team
        int randomTeam = Random.Range(0, teams.Count);
        if(teams[randomTeam].playersInTeam.Count < teams[randomTeam].maxPlayersInTeam)
        {
            teams[randomTeam].playersInTeam.Add(p.gameObject);
            p.GetComponent<playerStats>().teamId.Value = randomTeam; 
        }
        else
        {
            for (int i = 0; i < teams.Count; i++)
            {
                if(teams[i].playersInTeam.Count < teams[i].maxPlayersInTeam)
                teams[i].playersInTeam.Add(p.gameObject);
                p.GetComponent<playerStats>().teamId.Value = i; 
            }
        }
    }
}

[System.Serializable]
public class Teams
{
    public Color teamColor;
    public int maxPlayersInTeam;
    [HideInInspector] public List<GameObject> playersInTeam = new List<GameObject>();
}