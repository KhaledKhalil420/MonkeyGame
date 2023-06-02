using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;

public class playerStats : NetworkBehaviour
{
    public NetworkVariable<int> teamId = new NetworkVariable<int>(
    0,    
    NetworkVariableReadPermission.Everyone, 
    NetworkVariableWritePermission.Server
    );

    public GameObject winScreen;
    public GameObject loseScreen;
    

    public void Update()
    {
        GetComponent<SpriteRenderer>().color = spawnManager.instance.teams[teamId.Value].teamColor;
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!IsLocalPlayer) return;

        if(other.CompareTag("winBanana"))
        {
            winServerRpc(teamId.Value);
        }
    }

    public override void OnNetworkSpawn()
    {
        if(IsLocalPlayer) winScreen.transform.parent.GetComponent<Canvas>().worldCamera = Camera.main;
        else Destroy(winScreen.transform.parent);
    }

    [ServerRpc(RequireOwnership = false)]
    void winServerRpc(int winnerTeamId)
    {
        winClientRpc(winnerTeamId);
    }

    [ClientRpc]
    void winClientRpc(int winnerTeamId)
    {
        if(teamId.Value == winnerTeamId) winScreen.SetActive(true);
        else loseScreen.SetActive(true);
    }

}
