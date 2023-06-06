using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EasyNetworkBehaviour : NetworkBehaviour
{
    #region Go Scene
    
    public void LoadSceneRpc(string LevelName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(LevelName, LoadSceneMode.Single);
    }

    public void RestartSceneRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    #endregion
}
