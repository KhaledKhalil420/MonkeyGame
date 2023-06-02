using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class playerStats : NetworkBehaviour
{
    public NetworkVariable<int> teamId = new NetworkVariable<int>(
    0,    
    NetworkVariableReadPermission.Everyone, 
    NetworkVariableWritePermission.Server
    );

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        GetComponent<SpriteRenderer>().color = spawnManager.instance.teams[teamId.Value].teamColor;
    }
}
