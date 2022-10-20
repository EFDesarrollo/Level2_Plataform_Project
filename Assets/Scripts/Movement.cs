using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Components
    [HideInInspector]
    public Rigidbody2D rb;
    //Movment
    [Header("Movment Options")]
    // Power of jump
    public float jumpVelocity = 5f;
    // Multiply gravity when character is falling down
    public float fallMultiplier = 2.5f;
    // When release jump button and are applying more gravity so our character dosen't jump quite as high.
    public float lowJumpMultiplier = 2f;
    // Inputs
    [Header("Input Options")]
    public string jumpInput;
    // Booleans
    [Header("Booleans")]
    // Is on Ground check
    public bool onGorund;
    //offset
    [Header("Collisions Option")]
    public LayerMask groundLayer;
    public float collisionRadius = 0.25f;
    [Header("Offsets")]
    public Vector2 bottomOffset;
    private Color debugCollisionColor = Color.red;

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
        // Mechanics
        Jump();
    }
    void FixedUpdate()
    {
        JumpFeeling();
    }
    /// <summary>
    /// Jump Function
    /// Every frame we are going to check Player's Input and if onGround
    /// If the player press [jumpInput]
    ///     Then apply to vector up a force [jumpVelocity]
    /// </summary>
    void Jump()
    {
        if (Input.GetButtonDown(jumpInput) && onGorund)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpVelocity;
        }
    }
    /// <summary>
    /// Jump Feel
    /// Everey Fixed frame we are going to check:
    /// - if our vertical motion is less than zero (if we are falling)
    ///     then aply a multiplier to our gravity
    /// - Else if we are jumping up and we get the check that not still holding the [jumpInput]
    ///     then apply adiciona gravity again to do low jump
    /// </summary>
    void JumpFeeling()
    {
        if (rb.velocity.y < -0.2f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if(rb.velocity.y > 0.2f && !Input.GetButton(jumpInput))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
    }
}
