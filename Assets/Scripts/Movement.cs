using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Components
    [HideInInspector]
    public Rigidbody2D rb;

    [Header("Movment Options")]
    public bool canMove = true;
    public bool canJump = true;
    // Power of walk
    public float walkVelocity = 5f;
    // Power of jump
    public float jumpVelocity = 5f;
    // Multiply gravity when character is falling down
    public float fallMultiplier = 2.5f;
    // When release jump button and are applying more gravity so our character dosen't jump quite as high.
    public float lowJumpMultiplier = 2f;

    [Header("Collisions Option")]
    public LayerMask groundLayer;
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset = new Vector2(0, -0.5f);
    private Color debugCollisionColor = Color.red;

    [Header("Input Options")]
    public string jumpAxis = "Jump";
    public string xAxis = "Horizontal";
    // Booleans
    [Header("Booleans")]
    public bool canWalkNow;
    /// Jump Bolleans
    // Is the player pressing JumpInput
    public bool canJumpNow;
    // Is on Ground check
    public bool onGorund;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // checks
        onGorund = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        JumpImputDetect();
        WalkInputDetect();
    }
    void FixedUpdate()
    {
        // Mechanics
        Jump();
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
    void Walk()
    {
        if (!canMove)
            return;
        if (canWalkNow)
        {
            //rb.AddForce(new Vector2(Input.GetAxis(xAxis) * walkVelocity, 0));
            rb.velocity = new Vector2(Input.GetAxis(xAxis) * walkVelocity, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

    }
    /// <summary>
    /// Jump Imput Detect Function.
    /// Every frame we are going to check Player's Input and if onGround
    ///     Then bollean [canJumpNow] will be true.
    /// This function communicates the [Input] of the user with the logic of the physics.
    /// Because the [Input] checks are performed in the [Update] method while the physics are handled in the [FixedUpdate] method.
    /// </summary>
    void JumpImputDetect()
    {
        if (Input.GetButtonDown(jumpAxis) && onGorund)
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
    void Jump()
    {
        if (!canJump)
            return;
        // Modify Rigidbody velocity directly is a bad physics handled.
        // Is better apply a force through unity's built in physics System.
        if (canJumpNow)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
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

    void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
    }
}
