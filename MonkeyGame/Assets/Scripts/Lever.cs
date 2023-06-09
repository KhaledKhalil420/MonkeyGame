using Unity.Netcode;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] private GameObject LinkedObject;
    private ITriggerable LinkedObjectITriggerable;

    private SpriteRenderer SpriteRenderer;
    private bool IsPlayerIn;

    private void Start()
    {
        LinkedObjectITriggerable = LinkedObject.GetComponent<ITriggerable>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            IsPlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            IsPlayerIn = false;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) & IsPlayerIn) 
        {
            TriggerForClients_ClientRpc();
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

        SpriteRenderer.flipX = !SpriteRenderer.flipX;
    }
}
