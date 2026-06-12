
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private BoxCollider2D playerCollider;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 80f;
    [SerializeField] private float airAcceleration = 30f;
    [SerializeField] private float airDeceleration = 40f;
    [SerializeField] private float maxSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [Range(0f, 1f)]
    [SerializeField] private float variableJumpCut = 0.4f;
    [SerializeField] private float jumpHoldDuration = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float lowJumpGravityMultiplier = 3f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.15f;

    [Header("Input Buffer")]
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Apex Modifier")]
    [SerializeField] private float apexThreshold = 0.1f;
    [SerializeField] private float apexGravityMultiplier = 0.5f;
    [SerializeField] private float apexSpeedBoost = 2f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Thresholds")]
    [SerializeField] private float moveThreshold = 0.1f;
    [SerializeField] private float dashDecelerationMultiplier = 0.5f;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashColliderShrinkYMultiplier = 0.5f;

    #endregion

    #region Private Fields

    private Vector2 moveInput;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool isGrounded;
    private bool jumpPressed;
    private bool isJumping;
    private bool isDashing;
    private float jumpHoldTimer;
    private float dashDurationTimer;
    private float dashCooldownTimer;
    private bool isSprinting;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    #endregion

    #region Unity Lifecycle (Self-Managed)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundCheck == null)
            groundCheck = transform;
        if (playerCollider == null)
            playerCollider = GetComponent<BoxCollider2D>();
        originalColliderSize = playerCollider.size;
        originalColliderOffset = playerCollider.offset;
    }

    #endregion

    #region Controller Callbacks (Called by PlayerController)

    public void UpdateTimers(float dt)
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;

            if (jumpBufferTimer > 0f)
            {
                jumpPressed = true;
                jumpBufferTimer = 0f;
            }
        }
        else
        {
            coyoteTimer -= dt;
        }

        jumpBufferTimer -= dt;

        if (isJumping)
        {
            if (jumpHoldTimer > 0f)
                jumpHoldTimer -= dt;
            else
                isJumping = false;
        }

        if (dashDurationTimer > 0f)
            dashDurationTimer -= dt;
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= dt;

        if (isDashing && dashDurationTimer <= 0f)
        {
            isDashing = false;
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
        }
    }

    public void MovementControl(CombatState combatState, float fixedDt)
    {
        Vector2 velocity = rb.linearVelocity;

        if (moveInput.x > moveThreshold)
            FacingRight = true;
        else if (moveInput.x < -moveThreshold)
            FacingRight = false;

        Vector3 localScale = transform.localScale;
        if ((FacingRight && localScale.x < 0f) || (!FacingRight && localScale.x > 0f))
        {
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        if (combatState.IsAttacking)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, combatState.HorizontalDrag * fixedDt);
            velocity.y = Mathf.MoveTowards(velocity.y, 0f, combatState.VerticalDrag * fixedDt);
        }
        else if (isDashing)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, deceleration * dashDecelerationMultiplier * fixedDt);
        }
        else
        {
            float speedMultiplier = isSprinting ? sprintSpeedMultiplier : 1f;
            float currentMaxSpeed = maxSpeed * speedMultiplier;
            float targetSpeed = moveInput.x * moveSpeed * speedMultiplier;
            float accel = isGrounded ? acceleration : airAcceleration;
            float decel = isGrounded ? deceleration : airDeceleration;

            if (moveInput.x != 0f)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, accel * fixedDt);
                velocity.x = Mathf.Clamp(velocity.x, -currentMaxSpeed, currentMaxSpeed);
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, 0f, decel * fixedDt);
            }
        }

        if (jumpPressed)
        {
            velocity.y = jumpForce;
            jumpPressed = false;
            isJumping = true;
            jumpHoldTimer = jumpHoldDuration;
            coyoteTimer = 0f;
        }

        float gravityMultiplier = gravityScale;

        if (combatState.IsAttacking)
        {
            gravityMultiplier *= combatState.FallGravityMultiplier;
        }
        else if (!isGrounded)
        {
            bool isFalling = velocity.y < 0f;
            bool isApex = Mathf.Abs(velocity.y) < apexThreshold;

            if (isApex && !isFalling)
            {
                gravityMultiplier *= apexGravityMultiplier;

                if (moveInput.x != 0f)
                    velocity.x += moveInput.x * apexSpeedBoost * fixedDt;
            }
            else if (isFalling)
            {
                gravityMultiplier *= fallGravityMultiplier;
            }
            else if (!isJumping)
            {
                gravityMultiplier *= lowJumpGravityMultiplier;
            }
        }

        velocity.y += Physics2D.gravity.y * gravityMultiplier * fixedDt;
        rb.linearVelocity = velocity;
    }

    public void OnMove(Vector2 value)
    {
        moveInput = value;
    }

    public void OnJumpPerformed()
    {
        jumpBufferTimer = jumpBufferTime;

        if (coyoteTimer > 0f && !isDashing)
            jumpPressed = true;
    }

    public void OnJumpCanceled()
    {
        if (isJumping && rb.linearVelocity.y > 0f)
        {
            Vector2 v = rb.linearVelocity;
            v.y *= variableJumpCut;
            rb.linearVelocity = v;
            isJumping = false;
        }
    }

    public void OnSprint(bool active)
    {
        isSprinting = active;
    }

    public void OnDash()
    {
        if (dashCooldownTimer <= 0f && isGrounded)
        {
            dashDurationTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
            isDashing = true;

            Vector2 dashDir = FacingRight ? Vector2.right : Vector2.left;
            rb.AddForce(dashDir * dashForce, ForceMode2D.Impulse);

            Vector2 newSize = originalColliderSize;
            newSize.y *= dashColliderShrinkYMultiplier;
            playerCollider.size = newSize;

            Vector2 newOffset = originalColliderOffset;
            newOffset.y -= (originalColliderSize.y - newSize.y) / 2f;
            playerCollider.offset = newOffset;
        }
    }

    public MovementState GetState()
    {
        return new MovementState(FacingRight, isDashing, isGrounded);
    }

    #endregion

    #region Public Properties

    public bool FacingRight { get; private set; } = true;

    #endregion

    #region Gizmos

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    #endregion
}
