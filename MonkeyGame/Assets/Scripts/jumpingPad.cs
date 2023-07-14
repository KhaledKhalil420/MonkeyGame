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
            StartCoroutine(CameraFollow.Instance.ShakeCamera(0.05f, 0.1f));
            other.GetComponent<PlayerMovement>().Jump(jumpForce);
            Anim.SetTrigger("Spring");
        }
    }
}
