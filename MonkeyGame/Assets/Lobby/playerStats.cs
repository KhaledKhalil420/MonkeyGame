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
            winServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void winServerRpc()
    {
        winClientRpc();
    }

    [ClientRpc]
    void winClientRpc()
    {

    }

}
