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

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        spawnManager.instance.handleGameMode();
    }

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
        Debug.Log("winServerRpc");
        winClientRpc(winnerId);
    }

    [ClientRpc]
    void winClientRpc(int winnerId)
    {
        Debug.Log("winClientRpc");
        handleWin(winnerId);
    }


    void handleWin(int winnerId)
    {
        Debug.Log("handleWin");
        if((int)OwnerClientId == winnerId)
        {
            winScreen = spawnManager.instance.winPanel;
            winScreen.SetActive(true);
            winScreen.transform.GetChild(1).GetComponent<TMP_Text>().text = lobbyManager.Instance.currentLobby.Players[winnerId].Data["playerName"].Value + " win!";
        }
        if((int)OwnerClientId != winnerId)
        {
            winScreen = spawnManager.instance.winPanel;
            loseScreen.SetActive(true);
            loseScreen.transform.GetChild(1).GetComponent<TMP_Text>().text = lobbyManager.Instance.currentLobby.Players[winnerId].Data["playerName"].Value + " win!";
        }
        
    }
}
