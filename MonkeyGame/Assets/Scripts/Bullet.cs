using Unity.Netcode;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float CannonBallLifeSpan;
    [SerializeField] private float CannonBallSpeed;

    // Start is called before the first frame update
    void Start()
    {
        //Add Bullet Behaviour
        Rigidbody2D Rb = GetComponent<Rigidbody2D>();
        Rb.velocity += new Vector2(transform.right.x * CannonBallSpeed, Rb.velocity.y);
        
        Destroy(gameObject, CannonBallLifeSpan); 
    }
}
