using Unity.Netcode;
using System.Collections;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public static PlayerMovement Instance;
    
    //RAYCASTS
    private bool CanJump = true; 
    public bool IsGrounded;
    private bool IsWalled, IsReadyToJump; 

    [SerializeField] private Transform Feet;
    [SerializeField] private Transform WallPoint;
    [SerializeField] private Vector2 WallJumpDetection;

    [SerializeField] private LayerMask GroundLayer;
    public float MovementX, LastDirection;

    //Speed
    public float Speed;

    //Jump
    public float JumpForce, JumpCoolDown;

    //Wall Jumping
    [SerializeField] private float WallJumpingTime;
    [SerializeField] private float WallJumpingForceTime;
    [SerializeField] private Vector2 WallJumpingForce;
    private float WallJumpingTimer;
    private bool IsWallJumping;
    
    //Wall Sliding
    [SerializeField] private float WallSidingSpeed;
    private bool IsSliding; 
    private bool CanSlide = true;

    //Cayote Jump and Gravity
    [SerializeField] private float GravityOnGround, GravityLowJump;
    [SerializeField] private float CayoteJumpCoolDown;
    private float CayoteTimer;
    private bool CanCayoteJump;

    private Animator Anim;
    private Rigidbody2D Rb;       

    [SerializeField] private bool IsOffline;

    //particles
    [SerializeField] ParticleSystem dust;


    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        Rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        playDust();

        if(!IsClient & !IsOffline) return;

        if(Instance == null) Instance = this;
        
        Inputs();
        CayoteJump();
        DrawRayCasts();
        if(Anim != null) SetAnimations();

        WallJump();
    }



    void FixedUpdate()
    {
        WallSlide();
        ApplyMovement();   
    }
    

    void Inputs()
    {   
        //Get Movement Direction
        MovementX = Input.GetAxisRaw("Horizontal");
        
        //Get LastMovment Direction
        if(MovementX == 1 ) 
        {
            LastDirection = 1;
            Invoke(nameof(StopWallJump), WallJumpingForceTime);
        }

        if(MovementX == -1) 
        {
            LastDirection = -1;
            Invoke(nameof(StopWallJump), WallJumpingForceTime);
        }
        
        //Jumping When IsGrounded or can cayoteJump
        if(Input.GetButtonDown("Jump") & IsGrounded & CanJump | Input.GetButtonDown("Jump") & CanJump & CanCayoteJump)
        {
            //Jump
            Jump(JumpForce);

            //Reset Jump
            CanJump = false;
            Invoke(nameof(GetReadyToJump), JumpCoolDown);
        }
    
        
        //When releases space adds force DownWards
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

    bool canDust;
    void playDust()
    {
        if(IsGrounded)
        {
            if(canDust)
            {
                dust.Play();
                canDust = false;
            }
        }
        else
        canDust = true;
    }

    void ApplyMovement()
    {
        if(!IsWallJumping)
        Rb.velocity = new Vector2(MovementX * Speed * Time.fixedDeltaTime, Rb.velocity.y);
    }
    
    #region  WallJump

    void WallSlide()
        {
            if(IsWalled & !IsGrounded && MovementX != 0 & CanSlide)
            {
                IsSliding = true;
                if(Input.GetKey(KeyCode.LeftShift))
                Rb.velocity = new Vector2(Rb.velocity.x, Mathf.Clamp(Rb.velocity.y, -WallSidingSpeed, float.MaxValue) * Time.fixedDeltaTime);
            }
            else
            {
                IsSliding = false;
        }
    }

    void WallJump()
    {
        if(IsSliding & CanSlide)
        {
            IsWallJumping = false; 
            WallJumpingTimer = WallJumpingTime;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                IsWallJumping = true;
                Rb.velocity = new Vector2(-LastDirection * WallJumpingForce.x, WallJumpingForce.y) * Time.fixedDeltaTime;
           
                float Angle = (transform.eulerAngles.y == 0 ? 180 : 0);
                transform.eulerAngles = new Vector2(transform.eulerAngles.x, Angle);
                
                Invoke(nameof(StopWallJump), WallJumpingTime);
                Invoke(nameof(ResetSliding), 0.1f);
                CanSlide = false;
            }


            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            WallJumpingTimer -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && WallJumpingTimer > 0f)
        {
            IsWallJumping = true;
            Rb.velocity = new Vector2(-LastDirection * WallJumpingForce.x, WallJumpingForce.y) * Time.fixedDeltaTime;
            WallJumpingTimer = 0;
           
            float Angle = (transform.eulerAngles.y == 0 ? 180 : 0);
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, Angle);

            Invoke(nameof(StopWallJump), WallJumpingTime);
        }

        if(IsGrounded)
        {
            StopWallJump();
        }
    }

    void StopWallJump()
    {
        IsWallJumping = false;
    }

    void ResetSliding()
    {
        CanSlide = true;
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

        Gizmos.DrawWireCube(Feet.position, new Vector2(Bc.size.x - 0.05f, 0.2f));
        Gizmos.DrawWireCube(WallPoint.position, WallJumpDetection);
    }
    
    
    void DrawRayCasts()
    {
        //Get boxColllider
        BoxCollider2D Bc = GetComponent<BoxCollider2D>();

        ///Make GroundCheck
        Vector2 GroundCheckSize = new Vector2(Bc.size.x - 0.05f, 0.2f);
        IsGrounded = Physics2D.OverlapBox(Feet.position, GroundCheckSize, 0, GroundLayer);

        //When Wall detection
        IsWalled = Physics2D.OverlapBox(WallPoint.position, WallJumpDetection, 0, GroundLayer);
        
        //if Isgrounded
        if(IsGrounded) Rb.gravityScale = GravityOnGround;
    }

    #endregion
}