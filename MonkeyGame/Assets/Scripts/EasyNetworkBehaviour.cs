using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EasyNetworkBehaviour : NetworkBehaviour
{
    #region Destroy Object

    public void DestroyNObject(GameObject _gameObject)
    {
        Destroy_ServerRpc(_gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    void Destroy_ServerRpc(GameObject _gameObject)
    {
        Destroy_ClientRpc(_gameObject);
    }

    [ClientRpc]
    public void Destroy_ClientRpc(GameObject _gameObject)
    {
        _gameObject.GetComponent<NetworkObject>().Despawn(false);
        Destroy(_gameObject);
    }

    #endregion

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
