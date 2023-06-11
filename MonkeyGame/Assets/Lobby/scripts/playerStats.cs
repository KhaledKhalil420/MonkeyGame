using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;
using TMPro;

public class playerStats : NetworkBehaviour
{
    public SpriteRenderer outLine;

    public NetworkVariable<int> teamId = new NetworkVariable<int>(
    0,    
    NetworkVariableReadPermission.Everyone, 
    NetworkVariableWritePermission.Server
    );

    public TMP_Text nameText;

    GameObject winScreen;
    GameObject loseScreen;

    public void Update()
    {
        //outLine.material = spawnManager.instance.teams[teamId.Value].teamColor;
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!IsOwner) return;

        if(other.CompareTag("winBanana"))
        {
            spawnManager.instance.winManagerServerRpc((int)OwnerClientId);
            Debug.Log("hit banana");
        }
    }

    public override void OnNetworkSpawn()
    {
        nameText.text = lobbyManager.Instance.currentLobby.Players[(int)NetworkManager.LocalClientId].Data["playerName"].Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void winServerRpc(int winnerId)
    {
        winClientRpc(winnerId);
    }

    [ClientRpc]
    void winClientRpc(int winnerId)
    {
        handleWin(winnerId);
    }


    void handleWin(int winnerId)
    {
        Debug.Log("handleWin");
        if((int)OwnerClientId == winnerId)
        {
            winScreen = GameObject.FindGameObjectWithTag("win");
            winScreen.SetActive(true);
            winScreen.transform.GetChild(1).GetComponent<TMP_Text>().text = lobbyManager.Instance.currentLobby.Players[winnerId].Data["playerName"].Value + "win!";
        }
        if((int)OwnerClientId != winnerId)
        {
            winScreen = GameObject.FindGameObjectWithTag("lose");
            loseScreen.SetActive(true);
            loseScreen.transform.GetChild(1).GetComponent<TMP_Text>().text = lobbyManager.Instance.currentLobby.Players[winnerId].Data["playerName"].Value + "win!";
        }
        
    }
}
