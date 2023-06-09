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
        GetComponent<Rigidbody2D>().velocity += new Vector2(CannonBallSpeed * Time.fixedDeltaTime, 0);
        Destroy(gameObject, CannonBallLifeSpan);   
    }
}
