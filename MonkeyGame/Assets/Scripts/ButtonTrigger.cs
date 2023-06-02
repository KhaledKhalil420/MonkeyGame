using Unity.Netcode;
using UnityEngine;

public class ButtonTrigger : NetworkBehaviour
{
    [SerializeField] private GameObject LinkedObject;
    private ITriggerable LinkedObjectITriggerable;

    private void Start()
    {
        LinkedObjectITriggerable = LinkedObject.GetComponent<ITriggerable>();
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.CompareTag("Player"))
        {
            TriggerLikedObj_ServerRpc();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.collider.CompareTag("Player"))
        {
            TriggerLikedObj_ServerRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void TriggerLikedObj_ServerRpc()
    {
        TriggerForClients_ClientRpc();
    }
    
    [ClientRpc]
    void TriggerForClients_ClientRpc()
    {
        if(LinkedObjectITriggerable != null)
        LinkedObjectITriggerable.Trigger();
    }
}
