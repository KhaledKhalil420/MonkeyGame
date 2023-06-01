using Unity.Netcode;
using System.Collections;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private GameObject CameraObject;
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
        Instantiate(CameraObject, transform.position, Quaternion.identity);
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
        //Get Movement Direction
        MovementX = Input.GetAxisRaw("Horizontal");
        
        //Get LastMovment Direction
        if(MovementX == 1 & !DoingWallForce) LastDirection = 1;
        if(MovementX == -1 & !DoingWallForce) LastDirection = -1; 
        
        //Jumping When IsGrounded or can cayoteJump
        if(Input.GetButtonDown("Jump") & IsGrounded & CanJump & !IsWalled | Input.GetButtonDown("Jump") & CanJump & CanCayoteJump  & !IsWalled )
        {
            Jump(JumpForce);
            CanJump = false;
            Invoke(nameof(GetReadyToJump), JumpCoolDown);
        }
        
        //WallJump
        if(Input.GetButtonDown("Jump") & IsWalled & MovementX != 0 & !DoingWallForce)
        {
            StartCoroutine(WallForce());
        }
        
        //When releases space adds force downards
        if(Input.GetButtonUp("Jump") & !IsWalled)
        {
            if(Rb.velocity.y > 0)
            {
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
        //Set Movement Animations
        Anim.SetBool("IsGrounded", IsGrounded);
        Anim.SetBool("IsMoving", MovementX != 0);
        
        //Set Player flip angle
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
    
    #region  WallJump

    public void WallJump()
    {
        //If Is not walled change gravity do Normal gravity
        bool CanChangeGravity = new();
        if(IsWalled & !IsGrounded & MovementX != 0 )
        {
            Rb.gravityScale = GravityOnWall;
            CanChangeGravity = true;
        }
        else if(!IsWalled & CanChangeGravity)
        {
            Rb.gravityScale = GravityOnGround;
            CanChangeGravity = false;
        }
        
        //Adds WallJumpForce
        if(DoingWallForce & IsWalled)
        {
            Rb.velocity = new Vector2(-LastDirection * WallJumpXForce * Time.fixedDeltaTime, WallJumpYForce * Time.fixedDeltaTime);
        }
        
        //If It was still adding WallForce but Its not walled, flip the player
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

    #endregion
    
    #region Jumping

    public void Jump(float JumpForce)
    {
        //Reset Jump Then Jump
        Rb.velocity = new Vector2(Rb.velocity.x, 0);
        Rb.velocity = new Vector2(Rb.velocity.x, JumpForce * Time.fixedDeltaTime);
    }

    void GetReadyToJump()
    {
        //Reset CanJump
        CanJump = true;
    }

    void CayoteJump()
    {
        //When can Jump reset Cayote Timer
        if(IsGrounded & CanJump)
        CayoteTimer = CayoteJumpCoolDown;
        
        //When isn't grounded subtract timer for detatime;
        else
        CayoteTimer -= Time.deltaTime;
        
        //CanCayoteJump if timer was bigger than 0
        CanCayoteJump = CayoteTimer > 0;
    }

    #endregion

    #region  RayCasts

    private void OnDrawGizmosSelected()
    {
        //Get boxColllider
        BoxCollider2D Bc = GetComponent<BoxCollider2D>();

        Gizmos.DrawWireCube(Feet.position, new Vector2(Bc.size.x / 2 - 0.05f, 0.2f));
        Gizmos.DrawWireCube(WallPoint.position, WallJumpDetection);
    }
    
    
    void DrawRayCasts()
    {
        //Get boxColllider
        BoxCollider2D Bc = GetComponent<BoxCollider2D>();

        ///Make GroundCheck
        Vector2 GroundCheckSize = new Vector2(Bc.size.x / 2 - 0.05f, 0.2f);
        IsGrounded = Physics2D.OverlapBox(Feet.position, GroundCheckSize, 0, GroundLayer);

        //When Wall detection
        IsWalled = Physics2D.OverlapBox(WallPoint.position, WallJumpDetection, 0, GroundLayer);
        
        //if Isgrounded
        if(IsGrounded) Rb.gravityScale = GravityOnGround;
    }

    #endregion
}