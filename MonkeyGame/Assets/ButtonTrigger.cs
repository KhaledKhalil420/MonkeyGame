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
            TriggerLikedObj_ClientRpc();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.collider.CompareTag("Player"))
        {
            TriggerLikedObj_ClientRpc();
        }
    }


    [ClientRpc]
    void TriggerLikedObj_ClientRpc()
    {
        Debug.Log("w");
        if(LinkedObjectITriggerable != null)
        LinkedObjectITriggerable.Trigger();
    }
}
