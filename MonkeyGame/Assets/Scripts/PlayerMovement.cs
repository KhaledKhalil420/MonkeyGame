using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [Header("Physics, Jump"), Space(3)]                               
    bool CanJump = true; 
    public bool IsGrounded, IsReadyToJump;

    [Header("RayCast, Points"), Space(3)]
    public Transform Feet;

    [Header("RayCast, Layers"), Space(3)]
    public LayerMask GroundLayer;
    public float MovementX, LastDirection;

    [Header("Physics, Forces"), Space(3)]
    //Speed
    public float Speed;

    //Jump
    public float JumpForce;
    public float JumpCoolDown;

    [Header("Gamefeel, CayoteJump"), Space(3)]
    public float CayoteJumpCoolDown;
    float CayoteTimer;
    bool CanCayoteJump;

    [Header("Refrences"), Space(3)]
    Animator Anim;
    Rigidbody2D Rb;       


    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        Rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if(Instance == null) Instance = this;
        
        Inputs();
        CayoteJump();
        DrawRayCasts();
        if(Anim != null) SetAnimations();
    }



    void FixedUpdate()
    {
        if(GetComponent<PlayerVine>().IsAddingForce) 
        return;

        ApplyMovement();   
    }
    

    void Inputs()
    {
        if(GetComponent<PlayerVine>().IsAddingForce) 
        return;
        
        MovementX = Input.GetAxisRaw("Horizontal");
        if(MovementX == 1) LastDirection = 1;
        if(MovementX == -1) LastDirection = -1; 
        

        if(Input.GetButtonDown("Jump") & IsGrounded & CanJump | Input.GetButtonDown("Jump") & CanJump & CanCayoteJump)
        {
            Jump(JumpForce);
            CanJump = false;
            Invoke(nameof(GetReadyToJump), JumpCoolDown);
        }

        if(Input.GetButtonUp("Jump"))
        {
            if(Rb.velocity.y > 0)
            {
                Rb.velocity = new Vector2(Rb.velocity.x, 0);
                Rb.gravityScale = 3;
            }
            else if(Rb.velocity.y < 0)
            {
                Rb.gravityScale = 3;
            }
        }
    }
        
    void SetAnimations()
    {
        Anim.SetBool("IsGrounded", IsGrounded);
        Anim.SetBool("IsMoving", MovementX != 0);

        if(MovementX == 0) return;
        bool FlipS = (MovementX == -1 ? true : false);
        float Angle = (FlipS ? 180 : 0);

        transform.eulerAngles = new Vector2(transform.eulerAngles.x, Angle);
    }

    void ApplyMovement()
    {
        Rb.velocity = new Vector2(MovementX * Speed * Time.fixedDeltaTime, Rb.velocity.y);
    }

    public void Jump(float JumpForce)
    {
        Rb.velocity = new Vector2(Rb.velocity.x, 0);
        Rb.velocity = new Vector2(Rb.velocity.x, JumpForce * Time.fixedDeltaTime);
    }
    
    void GetReadyToJump()
    {
        CanJump = true;
    }

    void CayoteJump()
    {
        if(IsGrounded & CanJump)
        CayoteTimer = CayoteJumpCoolDown;

        else
        CayoteTimer -= Time.deltaTime;

        CanCayoteJump = CayoteTimer > 0;
    }


    private void OnDrawGizmosSelected()
    {
        BoxCollider2D Bc = GetComponent<BoxCollider2D>();
        Gizmos.DrawWireCube(Feet.position, new Vector2(Bc.size.x / 2 - 0.05f, 0.2f));
    }
    
    
    void DrawRayCasts()
    {
        BoxCollider2D Bc = GetComponent<BoxCollider2D>();
        Vector2 GroundCheckSize = new Vector2(Bc.size.x - 0.05f, 0.2f);
        IsGrounded = Physics2D.OverlapBox(Feet.position, GroundCheckSize, 0, GroundLayer);

        if(IsGrounded) Rb.gravityScale = 2;
    }
}