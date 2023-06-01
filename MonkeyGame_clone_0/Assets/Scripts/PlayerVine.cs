using UnityEngine.U2D;
using UnityEngine;
using System.Collections;

public class PlayerVine : MonoBehaviour
{
    private Rigidbody2D Rb;
    private DistanceJoint2D Joint;
    private PlayerMovement Movement;
    private bool CanEditVinePos, CanUseVine = true;
    

    public static bool IsUsingVine;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float JumpVel, JumpTime;
    [HideInInspector] public bool IsAddingForce = false;

    public bool addingfor;

    private void Start()
    {
        Movement = GetComponent<PlayerMovement>();
        Joint = GetComponent<DistanceJoint2D>();
        Rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Vine") & CanUseVine)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if(IsUsingVine)
                {
                    IsUsingVine = true;
                }
                else
                {
                    IsUsingVine = false;
                }
            }
        }
    }

    private void Update()
    {
        addingfor = IsAddingForce;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(IsUsingVine & !CanUseVine)
            {
                IsUsingVine = !IsUsingVine;
                CanUseVine = false;

                Rb.gravityScale = 2;
                Rb.velocity = new Vector2(Rb.velocity.x, JumpVel * Time.fixedDeltaTime);
                StartCoroutine(StartJumpDash());
                Invoke(nameof(CanUseVineReset), 0.3f);
            }
        }

        if(IsUsingVine & CanEditVinePos)
        {
            Rb.gravityScale =  Rb.gravityScale * 20;
            CanEditVinePos = false;
        }

        else if(!IsUsingVine)
        {
            CanEditVinePos = true;
            Joint.enabled = false;
        }
    }

    IEnumerator StartJumpDash()
    {
        IsAddingForce = true;
        yield return new WaitForSeconds(JumpTime);
        IsAddingForce = false;
    }

    private void FixedUpdate()
    {
        if(IsAddingForce)
        {
            if(Movement.IsGrounded) IsAddingForce = false; 
            Rb.velocity = new Vector2(Movement.LastDirection * JumpVel * Time.fixedDeltaTime, Rb.velocity.y);
        }
    }

    void CanUseVineReset()
    {
        CanUseVine = true;
    }
}