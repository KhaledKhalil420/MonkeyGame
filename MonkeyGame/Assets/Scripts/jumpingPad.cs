using UnityEngine;

public class jumpingPad : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    private Animator Anim;

    private void Start()
    {
        Anim = GetComponentInChildren<Animator>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().Jump(jumpForce);
            Anim.SetTrigger("Spring");
        }
    }
}
