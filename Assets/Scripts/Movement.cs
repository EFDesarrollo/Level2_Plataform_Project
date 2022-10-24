using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Components
    [HideInInspector]
    public Rigidbody2D rb;
    [Header("Stats")]
    // power of walk
    public float speed = 10;
    // Power of jump
    public float jumpForce = 50;
    // Multiply gravity when character is falling down
    public float fallMultiplier = 2.5f;
    // When release jump button and are applying more gravity so our character dosen't jump quite as high.
    public float lowJumpMultiplier = 2f;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    [Header("Movment Options")]
    public bool canMove = true;
    public bool canJump = true;

    [Header("Collisions Option")]
    public LayerMask groundLayer;
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset = new Vector2(0, -0.5f);
    public Vector2 leftOffset = new Vector2(-0.5f, 0);
    public Vector2 rigthOffset = new Vector2(0.5f, 0);
    private Color debugCollisionColor = Color.red;

    [Header("Input Options")]
    public string jumpAxis = "Jump";
    public string xAxis = "Horizontal";
    public string yAxis = "Vertical";
    // Booleans
    [Header("Booleans")]
    public bool canWalkNow;
    /// Jump Bolleans
    // Is the player pressing JumpInput
    public bool canJumpNow;
    // Is on Ground check
    public bool onGround, onWall, onLeftWall, onRightWall;
    public bool wallGrab, wallSlide, wallJumped;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // checks
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + rigthOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rigthOffset, collisionRadius, groundLayer);
        JumpImputDetect();
        WalkInputDetect();
        if (onWall && Input.GetButton("Fire3") && canMove)
        {
            wallGrab = true;
            wallSlide = false;
        }
        if (Input.GetButtonUp("Fire3") || !onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }
        if (onWall && !onGround)
        {
            if (Input.GetAxis(xAxis) != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }
        if (onGround)
        {
            wallJumped = false;
        }
        if (!onWall || onGround)
            wallSlide = false;
        // Mechanics
        WallGrab();
    }
    void FixedUpdate()
    {
        // Mechanics
        if (onGround)
            Jump(Vector2.up, false);
        if (onWall && !onGround)
            WallJump();
        Walk();
    }
    /// <summary>
    /// Jump Imput Detect Function.
    /// Every frame we are going to check Player's Input
    ///     Then bollean [canWalkNow] will be true.
    ///     This function communicates the [Input] of the user with the logic of the physics.
    /// Because the [Input] checks are performed in the [Update] method while the physics are handled in the [FixedUpdate] method.
    /// </summary>
    void WalkInputDetect()
    {
        canWalkNow = Input.GetButton(xAxis);
    }
    /// <summary>
    /// Walk Mechanic.
    /// Everey Fixed frame we are going to check:
    /// - If the player Can Walk (pressed Walk Input)
    ///     Then modify Rigidbody velocity [walkVelocity] to vector X.
    /// </summary>
    void Walk()
    {
        if (!canMove)
            return;
        if (wallGrab)
            return;
        if (!wallJumped)
        {
            if (canWalkNow)
            {
                //rb.AddForce(new Vector2(Input.GetAxis(xAxis) * walkVelocity, 0));
                rb.velocity = new Vector2(Input.GetAxis(xAxis) * speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(Input.GetAxis(xAxis) * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }

    }
    /// <summary>
    /// Jump Imput Detect Function.
    /// Every frame we are going to check Player's Input
    ///     Then bollean [canJumpNow] will be true.
    /// This function communicates the [Input] of the user with the logic of the physics.
    /// Because the [Input] checks are performed in the [Update] method while the physics are handled in the [FixedUpdate] method.
    /// </summary>
    void JumpImputDetect()
    {
        if (Input.GetButtonDown(jumpAxis))
            canJumpNow = true;
    }
    /// <summary>
    /// Jump Feel.
    /// Everey Fixed frame we are going to check:
    /// - If the player Can jump (pressed jump Input and is on Ground)
    ///     Then apply a force [jumpVelocity] to vector up.
    /// - If our vertical motion is less than zero (if we are falling)
    ///     then aply a multiplier to our gravity.
    /// - Else if we are jumping up and we get the check that not still holding the [jumpInput]
    ///     then apply adiciona gravity again to do low jump.
    /// - Else if not Falling and not Jumping
    ///     Then back to normal gravity.
    /// </summary>
    void Jump(Vector2 dir, bool wall)
    {
        if (!canJump)
            return;
        // Modify Rigidbody velocity directly is a bad physics handled.
        // Is better apply a force through unity's built in physics System.
        if (canJumpNow)
        {
            Debug.Log("Jumping");
            GetComponent<Rigidbody2D>().AddForce(dir * jumpForce, ForceMode2D.Impulse);
            canJumpNow = false;
        }
        if (rb.velocity.y < -0.2f)
        {
            rb.gravityScale = fallMultiplier;
        } else if(rb.velocity.y > 0.2f && !Input.GetButton(jumpAxis))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            // Back to normal gravity
            rb.gravityScale = 1f;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private void WallJump()
    {
        
        //StopCoroutine(DisableMovement(0));
        //StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }
    /// <summary>
    /// 
    /// </summary>
    void WallGrab()
    {
        if (wallGrab)
        {
            float speedModifier = Input.GetAxis(yAxis) > 0 ? .5f : 1;

            rb.gravityScale = 0;
            if (Input.GetAxis(xAxis) > .2f || Input.GetAxis(xAxis) < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        else
        {
            rb.gravityScale = 3;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void WallSlide()
    {
        if (!canMove)
            return;
        
        bool pushingWall = false;
        if ((rb.velocity.x > 0 && onRightWall) || (rb.velocity.x < 0 && onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rigthOffset, collisionRadius);
    }
}
