using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    /// Variables related to the components of the object
    /// 
    /// <value>Stores the Rigidbody component of the object.</value>
    [HideInInspector]
    public Rigidbody2D rb;

    /// Variables related to player stats
    /// 
    /// <value>Reepresent power/Speed of walk.</value>
    [Header("Stats")]
    public float speed = 10;
    /// <value>Reepresent Power of jump.</value>
    public float jumpForce = 12;
    /// <value>Multiply gravity when character is falling down</value>
    public float fallMultiplier = 2.5f;
    /// <value>When release jump button and are applying more gravity so our character dosen't jump quite as high.</value>
    public float lowJumpMultiplier = 2f;
    /// <value>constant slide speed in vector Y</value>
    public float slideSpeed = 1;
    /// <value>constant jump lerp</value>
    public float wallJumpLerp = 10;
    /// <value>Dash speed</value>
    public float dashSpeed = 20;

    /// Variables related to player movment
    /// 
    /// <value>Boolean that allow the mechanic of Movement</value>
    [Header("Movment Options")]
    public bool canMove = true;
    /// <value>Boolean that allow the mechanic of Jump</value>
    public bool canJump = true;

    /// Variables related to player collision
    /// 
    /// <value>Layer Mask tag that will be utilized to detect collisions</value>
    [Header("Collisions Option")]
    public LayerMask groundLayer;
    /// <value>Radius Distance to detect</value>
    public float collisionRadius = 0.25f;
    /// <value>Ground collision detector coordinates</value>
    public Vector2 bottomOffset = new Vector2(0, -0.3f);
    /// <value>Left collision detector coordinates</value>
    public Vector2 leftOffset = new Vector2(-0.3f, 0);
    /// <value>Rigth collision detector coordinates</value>
    public Vector2 rigthOffset = new Vector2(0.3f, 0);
    /// <value>color of the collision Gizmos in the scene</value>
    private Color debugCollisionColor = Color.red;

    /// Variables related to user input
    /// 
    /// <value>Jump Axis name</value>
    [Header("Input Options")]
    public string jumpAxis = "Jump";
    /// <value>X Axis name</value>
    public string xAxis = "Horizontal";
    /// <value>Y Axis name</value>
    public string yAxis = "Vertical";

    /// Boolean variables for detection functions
    /// 
    /// <value>Boolean expression that allows identifying if it meets the requirements to walk</value>
    [Header("Booleans")]
    public bool canWalkNow;
    /// <value>Boolean expression that allows identifying if it meets the requirements to Jump</value>
    public bool canJumpNow, isLeftJumpStored;
    /// <value>Boolean expression that allow to identify collisions</value>
    public bool onGround, onWall, onLeftWall, onRightWall;
    /// <value>Boolean expression that allows to identify if the mechanic is active</value>
    public bool wallGrab, wallSlide, wallJumped;

    [Header("Polish")]
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Detection functions
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + rigthOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rigthOffset, collisionRadius, groundLayer);
        JumpImputDetect();
        WalkInputDetect();
        WallGrabInputDetect();
        WallSlideDetect();
        WallJumpedDetect();
        // Particles
        JumpParticle();
        WallParticle(Input.GetAxis("Vertical"));

    }
    void FixedUpdate()
    {
        // Mechanics functions
        WallGrab();
        WallSlide();
        Walk();
        // Jump Logic
        if (onGround)
            Jump(Vector2.up, false);
        if (onWall)
            WallJump();
    }
    #region Detect
    /// <summary>
    /// Jump Imput Detect Function.
    /// <para>check the player's input on the X Axis, and change <see cref="canWalkNow"/> value</para>
    /// </summary>
    /// <remarks>
    /// This function communicates the [<see langword="Input"/>] of the user with the logic of the physics.
    /// Because the [<see langword="Input"/>] checks are performed in the [<see cref="Update"/>] method 
    /// while the physics are handled in the [<see cref="FixedUpdate"/>] method.
    /// </remarks>
    void WalkInputDetect()
    {
        canWalkNow = Input.GetButton(xAxis);
    }
    /// <summary>
    /// Jump Imput Detect Function.
    /// <para>
    /// check Player Input on jump Axis and change <see cref="canJumpNow"/> and <see cref="wallSlide"/> value.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This function communicates the [<see langword="Input"/>] of the user with the logic of the physics.
    /// Because the [<see langword="Input"/>] checks are performed in the [<see cref="Update"/>] method 
    /// while the physics are handled in the [<see cref="FixedUpdate"/>] method.
    /// </remarks>
    void JumpImputDetect()
    {
        if (Input.GetButtonDown(jumpAxis))
        {
            canJumpNow = true;
            wallSlide = false;
        }
    }
    /// <summary>
    /// Wall Grab Imput Detect Function.
    /// <para>
    /// check Player Input on Grab button and change <see cref="wallGrab"/> and <see cref="wallSlide"/> value.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This function communicates the [<see langword="Input"/>] of the user with the logic of the physics.
    /// Because the [<see langword="Input"/>] checks are performed in the [<see cref="Update"/>] method 
    /// while the physics are handled in the [<see cref="FixedUpdate"/>] method.
    /// </remarks>
    void WallGrabInputDetect()
    {
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
    }
    /// <summary>
    /// Wall slide Detect Function.
    /// <para>
    /// check if the Player is on a wall and his Input on X Axis, then modify [<see cref="wallSlide"/>] value.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This function communicates the [<see langword="Input"/>] of the user with the logic of the physics.
    /// Because the [<see langword="Input"/>] checks are performed in the [<see cref="Update"/>] method 
    /// while the physics are handled in the [<see cref="FixedUpdate"/>] method.
    /// </remarks>
    void WallSlideDetect()
    {
        if (onWall && !onGround)
        {
            if (Input.GetAxis(xAxis) != 0 && !wallGrab)
            {
                wallSlide = true;
            }
            else
            {
                wallSlide = false;
            }
        }
        if (!onWall || onGround)
            wallSlide = false;
    }
    /// <summary>
    /// Wall Jump Detect Function.
    /// <para>
    /// check if the Player was on a wall and isn't on the ground, then modify [<see cref="wallJumped"/>] value.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This function communicates the [<see langword="Input"/>] of the user with the logic of the physics.
    /// Because the [<see langword="Input"/>] checks are performed in the [<see cref="Update"/>] method 
    /// while the physics are handled in the [<see cref="FixedUpdate"/>] method.
    /// </remarks>
    void WallJumpedDetect()
    {
        if (onWall && !onGround)
        {
            wallJumped = true;
        }
        if (onGround)
        {
            wallJumped = false;
        }
    }
    #endregion

    #region Mechanics
    /// <summary>
    /// <para>Walk Mechanic.</para>
    /// <para>Everey Fixed frame we are going to check:</para>
    /// <list type="bullet">
    /// <item>If is allowed to move [<see cref="canMove"/>]</item>
    /// <item>If the mechanic [<see cref="wallGrab"/>] is active</item>
    /// <item>If the movment is not performed after a <see cref="wallJumped"/></item>
    /// <list type="bullet">
    /// <item>If meet the requirements to walk [<see cref="canWalkNow"/>]</item>
    /// <list type="bullet">
    /// <item>Then modify the speed of the <see langword="Rigidbody"/> in the vector x giving a 
    /// floating variable <see cref="speed"/>.</item>
    /// </list>
    /// <item>Else set <see cref="Rigidbody"/> speed to 0</item>
    /// </list>
    /// <item>If movment is performed after a <see cref="wallJumped"/></item>
    /// <item>Then modify the speed of the <see cref="Rigidbody"/> by doing a linear interpolation 
    /// [<see langword="Lerp"/>].</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// This method is closely related to the <see cref="WalkInputDetect"/> method that modify
    /// <see cref="canMove"/> value.
    /// <para>
    /// This method does modify the speed of the Rigidbody component directly 
    /// to avoid infinite acceleration.
    /// Is better apply a force through unity's built in physics System, 
    /// but it is necessary for this project requirements.
    /// </para>
    /// </remarks>
    void Walk()
    {
        // TODO: EXTRAER ESTA VARIABLE PARA QUE EL METODO SEA GLOBAL.
        float x = Input.GetAxis(xAxis);
        if (!canMove)
            // if not allowed to walk then retun
            return;
        if (wallGrab)
            // if mechanic wallGrab is active then return
            return;
        if (!wallJumped)
        {
            // if the move is not performed after a wall jump and
            if (canWalkNow)
                // if met the requirements to move then change Rigidbody velocity
                rb.velocity = new Vector2(x * speed, rb.velocity.y);
            else
                // if dosen't met the requirements to move then change Rigidbody velocity to zero
                rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
            // if the move is performed after a wall jump Then do a
            // linearly interpolate between actual x velocity an actual xvelocity * var velocity
            // by constant wallJumpLerp
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);

    }

    /// <summary>
    /// Jump Feel.
    /// <para>Everey Fixed frame we are going to check:</para>
    /// <list type="bullet">
    /// <item>If the player is allowed to jump [<see cref="canJump"/>]</item>
    /// <item>If the player mets the requirements to jump [<see cref="canJumpNow"/>]</item>
    /// <list type="bullet">
    /// <item>Then apply a force [<see cref="jumpForce"/>] to Y Axis.</item>
    /// </list>
    /// <item>If our vertical motion is less than zero [<see langword="we are falling"/>]</item>
    /// <list type="bullet">
    /// <item>then aply a multiplier [<see cref="fallMultiplier"/>] to the gravity scale from <see langword="Rigidbody"/>.</item>
    /// </list>
    /// <item>If our vertical motion is more than zero [<see langword="we are jumping"/>] but is not holding the <see cref="jumpAxis"/> button</item>
    /// <list type="bullet">
    /// <item>then apply a low gravity scale [<see cref="lowJumpMultiplier"/>] to do a low jump.</item>
    /// </list>
    /// <item>Else in any other case</item>
    /// <list type="bullet">
    /// <item>Then back to normal gravity.</item>
    /// </list>
    /// </list>
    /// </summary>
    /// <remarks>
    /// This method does not modify the speed of the Rigidbody component directly 
    /// because it is a bad practice for handling physics.
    /// Is better apply a force through unity's built in physics System, 
    /// It also makes the jump look more natural.
    /// </remarks>
    void Jump(Vector2 dir, bool wall)
    {
        if (!canJump)
            // If is not allowed to Jump then return.
            return;
        
        if (canJumpNow)
        {
            // If the Jump requirements are met then we apply a force on the Rigidbody in the Y axis.
            GetComponent<Rigidbody2D>().AddForce(dir * (wallJumped? jumpForce*2: jumpForce), ForceMode2D.Impulse);
            canJumpNow = false;
        }
        if (rb.velocity.y < -0.2f)
        {
            // If is falling then change gravity scale of rigidbody
            rb.gravityScale = fallMultiplier;
        } else if(rb.velocity.y > 0.2f && !Input.GetButton(jumpAxis))
        {
            // If are jumping, apply a low gravity scale if the jumpAxis button is not pressed
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            // in any other case Then gravity scale will be normal
            // this allow to do long jump while jumpAxis button is pressed
            rb.gravityScale = 1f;
        }
    }
    /// <summary>
    /// Wall Jump Mechanic
    /// <para>This method detect if the jump is made from a wall and call the <see cref="Jump(Vector2, bool)"/> method with a vector</para>
    /// </summary>
    private void WallJump()
    {
        //TODO: AÑADIR UN LIMITADOR DE SALTOS PARA QUE SALTE UNA VEZ DE CADA LADO
        Vector2 wallDir = onRightWall ? Vector2.left : Vector2.right;
        if (onWall && (isLeftJumpStored != onLeftWall || isLeftJumpStored != onLeftWall))
        {
            if (wallJumped && canJumpNow)
            {
                isLeftJumpStored = isLeftJumpStored && !onLeftWall? false : true;
                Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);
            }
        }
    }
    /// <summary>
    /// Wall Grab Mechanic
    /// <para>This method change the gravity scale to zero and set rigid vody velocity to zero</para>
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
    /// Wall Slide Mechanic
    /// <para>This method change the rigidbody velocity from Y Axis to <see cref="slideSpeed"/></para>
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

        if (wallSlide)
        {

            rb.velocity = new Vector2(push, -slideSpeed);
        }
    }
    #endregion
    void OnDrawGizmos()
    {
        
        Gizmos.color = debugCollisionColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rigthOffset, collisionRadius);

        Vector3 wallDir = onRightWall ? Vector3.left : Vector3.right;
        Debug.DrawRay(transform.position, Vector3.up / 1.5f + wallDir / 1.5f, Color.green);

    }
    void JumpParticle()
    {
        var wallJumpP = wallJumpParticle.main;
        var jumpP = jumpParticle.main;
        if (onGround || onWall)
        {
            wallJumpP.startColor = Color.clear;
            jumpP.startColor = Color.clear;
        }
        else
        {
            if (wallJumped)
                wallJumpP.startColor = Color.white;
            else
                jumpP.startColor = Color.white;
        }
    }
    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }
    int ParticleSide()
    {
        int particleSide = onLeftWall ? 1 : -1;
        return particleSide;
    }
}
