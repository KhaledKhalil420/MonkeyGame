using Unity.Netcode;
using UnityEngine;

public class DebugManager : NetworkBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            NetworkManager.Singleton.StartHost();
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
