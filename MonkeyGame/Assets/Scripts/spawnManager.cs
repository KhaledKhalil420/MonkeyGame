using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;

public class spawnManager : NetworkBehaviour
{
    public NetworkObject playerPrefab;
    public Transform[] spawnPoint;

    Lobby lobby;

    [Header("Teams")]

    public bool useTeams;

    public List<Teams> teams = new List<Teams>();

    public List<NetworkObject> players;

    [Header("UI")]
    public TMP_Text roundText;


    public enum gameMode {normal, flipGravity};
    [Header("Game mode")]
    public gameMode gamemode;

    public GameObject winPanel;
    public GameObject losePanel;

    public static spawnManager instance;
    

    void Start()
    {
        if(instance == null) instance = this;
        spawnPlayerServerRpc(NetworkManager.LocalClientId);

        roundText.text = "Round" + lobbyManager.Instance.curRound;
    }

    private void Update()
    {
        if(instance == null) instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    void spawnPlayerServerRpc(ulong id)
    {
        //spawn the player
        int randomSpawnPoint = Random.Range(0, spawnPoint.Length);
        NetworkObject p = NetworkManager.Instantiate(playerPrefab, spawnPoint[randomSpawnPoint].position, Quaternion.identity);
        p.SpawnAsPlayerObject((ulong)id);

        players.Add(p);

        if(!useTeams) return;
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

    public void handleGameMode()
    {
        GameObject localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;

        switch (gamemode)
        {
            case  gameMode.normal:
            break;

            case gameMode.flipGravity:
            localPlayerObject.GetComponent<Rigidbody2D>().gravityScale = -localPlayerObject.GetComponent<Rigidbody2D>().gravityScale;
            break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void winManagerServerRpc(int win)
    {
        foreach (NetworkObject player in players)
        {
            player.GetComponent<playerStats>().winServerRpc(win);
        }
    }
}

[System.Serializable]
public class Teams
{
    public Material teamColor;
    public int maxPlayersInTeam;
    [HideInInspector] public List<GameObject> playersInTeam = new List<GameObject>();
}