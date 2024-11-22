using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Animator animator;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float glideFallSpeed = 2f; // Fall speed during gliding
    public float fastFallSpeed = 10f; // Fall speed during fast fall
    private bool canJump = false;
    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;
    private bool canTripleJump = false;
    private bool hasTripleJumped = false;
    private bool tripleJumpAvailable = false;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isFacingRight = true;
    private Vector2 platformVelocity;

    private bool isOnLadder = false;
    private float ladderSpeed = 4f;
    private Collider2D currentLadder;

    private bool canGlide = true; // Can the player glide?
    private bool isGliding = false; // Is the player currently gliding?
    private bool isFastFalling = false; // Is the player currently fast-falling?

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if (!isOnLadder)
        {
            Vector2 adjustedVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y) + platformVelocity;
            rb.linearVelocity = adjustedVelocity;
            UpdateMovementAnimations(moveInput);
        }
        else
        {
            HandleLadderMovement();
        }

        // Reset grounded state if the player starts falling
        if (isGrounded && rb.linearVelocity.y < 0)
        {
            isGrounded = false;
        }

        // Check if the player has landed
        if (!isGrounded && rb.linearVelocity.y >= 0)
        {
            isGrounded = true;

            // Trigger landing animation
            animator.SetBool("IsLanding", true);

            // Stop gliding and fast-falling animations
            animator.SetBool("IsFlying", false);
            animator.SetBool("IsFastFalling", false);

            // Reset landing animation after a brief delay (handled in Animator)
            Invoke("ResetLandingAnimation", 0.1f);
        }

        // Glide logic: Always active when falling and gliding is enabled
        if (canGlide && !isGrounded && rb.linearVelocity.y < 0)
        {
            isGliding = true;

            // Check for fast fall input
            if (Input.GetKey(KeyCode.S))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);

                // Trigger fast-fall animation
                if (!isFastFalling)
                {
                    isFastFalling = true;
                    animator.SetBool("IsFastFalling", true);
                }

                // Ensure gliding animation is off while fast-falling
                if (animator.GetBool("IsFlying"))
                {
                    animator.SetBool("IsFlying", false);
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -glideFallSpeed);

                // Trigger gliding animation
                if (isFastFalling)
                {
                    isFastFalling = false;
                    animator.SetBool("IsFastFalling", false);
                }

                if (!animator.GetBool("IsFlying"))
                {
                    animator.SetBool("IsFlying", true);
                }
            }
        }
        else
        {
            if (isGliding)
            {
                // Stop gliding animation
                animator.SetBool("IsFlying", false);
            }
            isGliding = false;

            if (isFastFalling)
            {
                // Stop fast-fall animation
                isFastFalling = false;
                animator.SetBool("IsFastFalling", false);
            }
        }

        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    void HandleLadderMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * ladderSpeed);

        animator.SetFloat("speed", Mathf.Abs(horizontalInput * moveSpeed));
        animator.SetBool("IsClimbing", Mathf.Abs(verticalInput) > 0.1f);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetBool("IsJump", true);
        isGrounded = false;
    }

    void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.2f);
        hasDoubleJumped = true;
        animator.SetBool("IsJump", true);
        isGrounded = false;
    }

    void TripleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.5f);
        hasTripleJumped = true;
        tripleJumpAvailable = false;
        animator.SetBool("IsJump", true);
        isGrounded = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts.Any(contact => contact.normal.y > 0.5f))
        {
            if (!isGrounded)
            {
                isGrounded = true;
                animator.SetBool("IsJump", false);
                hasDoubleJumped = false;
                hasTripleJumped = false;
            }

            OscillatePlatform platform = collision.collider.GetComponentInParent<OscillatePlatform>();
            if (platform != null)
            {
                platformVelocity = platform.GetPlatformVelocity();
                return;
            }
        }

        platformVelocity = Vector2.zero;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<OscillatePlatform>() != null)
        {
            platformVelocity = Vector2.zero;
        }
    }

    void UpdateMovementAnimations(float moveInput)
    {
        animator.SetFloat("speed", Mathf.Abs(moveInput * moveSpeed));

        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void UnlockJump()
    {
        canJump = true;
        isGrounded = true;
    }

    public void EnableDoubleJump()
    {
        canDoubleJump = true;
    }

    public void EnableTripleJump()
    {
        canTripleJump = true;
        tripleJumpAvailable = true;
    }

    public void DisableGlide()
    {
        canGlide = false;
        animator.SetBool("IsFlying", false); // Ensure flying animation stops if gliding is disabled
        animator.SetBool("IsFastFalling", false); // Ensure fast-falling animation stops if gliding is disabled
    }

    void ResetLandingAnimation()
    {
        animator.SetBool("IsLanding", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
            currentLadder = other;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            isGrounded = true;
            hasDoubleJumped = false;
            hasTripleJumped = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder") && other == currentLadder)
        {
            isOnLadder = false;
            currentLadder = null;
            rb.gravityScale = 1;
        }
    }
}
