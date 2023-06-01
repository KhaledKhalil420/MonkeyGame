using Unity.Netcode;
using System.Collections;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public static PlayerMovement Instance;

    [Header("Physics, Jump"), Space(3)]                               
    bool CanJump = true; 
    public bool IsGrounded, IsWalled, IsReadyToJump;
    private bool DoingWallForce;


    [Header("RayCast, Points"), Space(3)]
    public Transform Feet;
    public Transform WallPoint;
    public Vector2 WallJumpDetection;

    [Header("RayCast, Layers"), Space(3)]
    public LayerMask GroundLayer;
    public float MovementX, LastDirection;

    [Header("Physics, Forces"), Space(3)]
    //Speed
    public float Speed;

    //Jump
    public float JumpForce;
    public float JumpCoolDown;
    public float WallJumpXForce, WallJumpYForce;
    public float WallJumpTime;

    [Header("GameFeel, Gravity")]
    public float GravityOnGround, GravityLowJump, GravityOnWall;

    [Header("Gamefeel, CayoteJump"), Space(3)]
    public float CayoteJumpCoolDown;
    float CayoteTimer;
    bool CanCayoteJump;

    [Header("Refrences"), Space(3)]
    Animator Anim;
    Rigidbody2D Rb;       

    [SerializeField] private bool IsOffline;


    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        
    }


    void Update()
    {
        if(!IsClient & !IsOffline) return;

        if(Instance == null) Instance = this;
        
        Inputs();
        CayoteJump();
        DrawRayCasts();
        if(Anim != null) SetAnimations();
    }



    void FixedUpdate()
    {
        ApplyMovement();   
        WallJump();
    }
    

    void Inputs()
    {   
        MovementX = Input.GetAxisRaw("Horizontal");
        
        if(MovementX == 1 & !DoingWallForce) LastDirection = 1;
        if(MovementX == -1 & !DoingWallForce) LastDirection = -1; 
        

        if(Input.GetButtonDown("Jump") & IsGrounded & CanJump | Input.GetButtonDown("Jump") & CanJump & CanCayoteJump)
        {
            Jump(JumpForce);
            CanJump = false;
            Invoke(nameof(GetReadyToJump), JumpCoolDown);
        }

        if(Input.GetButtonDown("Jump") & IsWalled & !IsGrounded & MovementX != 0 & !DoingWallForce)
        {
            StartCoroutine(WallForce());
        }

        if(Input.GetButtonUp("Jump"))
        {
            if(Rb.velocity.y > 0)
            {
                Rb.velocity = new Vector2(Rb.velocity.x, 0);
                Rb.gravityScale = GravityLowJump;
            }
            else if(Rb.velocity.y < 0)
            {
                Rb.gravityScale = GravityLowJump;
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
        if(!DoingWallForce)
        Rb.velocity = new Vector2(MovementX * Speed * Time.fixedDeltaTime, Rb.velocity.y);
    }

    public void Jump(float JumpForce)
    {
        Rb.velocity = new Vector2(Rb.velocity.x, 0);
        Rb.velocity = new Vector2(Rb.velocity.x, JumpForce * Time.fixedDeltaTime);
    }

    public void WallJump()
    {
        bool CanChangeGravity = new();
        if(IsWalled & !IsGrounded & MovementX != 0 )
        {
            Rb.gravityScale = GravityOnWall;
            CanChangeGravity = true;
        }
        else if(!IsWalled & IsGrounded & CanChangeGravity)
        {
            Rb.gravityScale = GravityOnGround;
            CanChangeGravity = false;
        }
        
        if(DoingWallForce & IsWalled)
        {
            Rb.velocity = new Vector2(-LastDirection * WallJumpXForce * Time.fixedDeltaTime, WallJumpYForce * Time.fixedDeltaTime);
        }

        else if(DoingWallForce & !IsWalled)
        {
            bool FlipS = (LastDirection == 1 ? true : false);
            float Angle = (FlipS ? 180 : 0);
            LastDirection = (FlipS ? -1 : 1);

            transform.eulerAngles = new Vector3(0, Angle, 0);
        }
    }

    IEnumerator WallForce()
    {
        DoingWallForce = true;

        yield return new WaitForSeconds(WallJumpTime);
        
        DoingWallForce = false;
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
        Gizmos.DrawWireCube(WallPoint.position, WallJumpDetection);
    }
    
    
    void DrawRayCasts()
    {
        BoxCollider2D Bc = GetComponent<BoxCollider2D>();
        Vector2 GroundCheckSize = new Vector2(Bc.size.x / 2 - 0.05f, 0.2f);
        IsGrounded = Physics2D.OverlapBox(Feet.position, GroundCheckSize, 0, GroundLayer);
        IsWalled = Physics2D.OverlapBox(WallPoint.position, WallJumpDetection, 0, GroundLayer);

        if(IsGrounded) Rb.gravityScale = GravityOnGround;
    }
}