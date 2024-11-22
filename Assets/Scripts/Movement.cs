using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Animator animator; // Reference to Animator component
    public float moveSpeed = 5f; // Speed for horizontal movement
    public float jumpForce = 10f; // Force applied for jumping
    private bool canJump = false; // Initially, the player cannot jump
    private bool canDoubleJump = false; // Can the player double jump?
    private bool hasDoubleJumped = false; // Has the double jump been used?
    private bool canTripleJump = false; // Can the player triple jump?
    private bool hasTripleJumped = false; // Has the triple jump been used?
    private bool tripleJumpAvailable = false; // Is triple jump power-up active?
    private Rigidbody2D rb; // Reference to Rigidbody2D
    private bool isGrounded; // Is the player on the ground?
    private bool isFacingRight = true; // Tracks the player's facing direction
    private Vector2 platformVelocity; // Tracks platform velocity for moving platforms

    private bool isOnLadder = false; // Is the player on a ladder?
    private float ladderSpeed = 4f; // Speed for ladder movement
    private Collider2D currentLadder; // Reference to the ladder the player is on

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Assign the Animator component
    }

    void Update()
    {
        // Handle movement
        float moveInput = Input.GetAxis("Horizontal");
        if (!isOnLadder) // Normal ground movement
        {
            Vector2 adjustedVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y) + platformVelocity;
            rb.linearVelocity = adjustedVelocity;

            // Update horizontal movement animations
            UpdateMovementAnimations(moveInput);
        }
        else // Ladder movement
        {
            HandleLadderMovement();
        }

        // Jump logic
        if (canJump && Input.GetButtonDown("Jump"))
        {
            if (isGrounded || isOnLadder) // Allow jumping on ground or ladder
            {
                Jump(); // Perform jump
                hasDoubleJumped = false; // Reset double jump
                hasTripleJumped = false; // Reset triple jump

                // Detach from ladder if jumping off
                if (isOnLadder)
                {
                    isOnLadder = false;
                    currentLadder = null;
                    rb.gravityScale = 1; // Restore gravity
                }
            }
            else if (canTripleJump && tripleJumpAvailable && !hasTripleJumped)
            {
                TripleJump(); // Perform triple jump
            }
            else if (canDoubleJump && !hasDoubleJumped)
            {
                DoubleJump(); // Perform double jump
            }
        }

        // Set animator vertical speed parameter
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    void HandleLadderMovement()
    {
        float verticalInput = Input.GetAxis("Vertical"); // W/S for climbing
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D for side-to-side movement

        // Combine horizontal and vertical ladder movement
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * ladderSpeed);

        // Update animations
        animator.SetFloat("speed", Mathf.Abs(horizontalInput * moveSpeed)); // Horizontal movement animation
        animator.SetBool("IsClimbing", Mathf.Abs(verticalInput) > 0.1f); // Climbing animation
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Apply jump force
        animator.SetBool("IsJump", true); // Trigger jump animation
        isGrounded = false; // Set grounded to false
    }

    void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.2f);
        hasDoubleJumped = true;
        animator.SetBool("IsJump", true); // Keep jump animation active
        isGrounded = false; // Set grounded to false
    }

    void TripleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.5f);
        hasTripleJumped = true;
        tripleJumpAvailable = false; // Expend the triple jump power-up
        animator.SetBool("IsJump", true); // Keep jump animation active
        isGrounded = false; // Set grounded to false
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts.Any(contact => contact.normal.y > 0.5f))
        {
            if (!isGrounded) // Ensure state transitions correctly
            {
                isGrounded = true;
                animator.SetBool("IsJump", false); // Stop jump animation when grounded
                hasDoubleJumped = false; // Reset double jump
                hasTripleJumped = false; // Reset triple jump
            }

            // Check if the player is standing on a moving platform
            OscillatePlatform platform = collision.collider.GetComponentInParent<OscillatePlatform>();
            if (platform != null)
            {
                platformVelocity = platform.GetPlatformVelocity();
                return;
            }
        }

        // If not on a platform, reset platform velocity
        platformVelocity = Vector2.zero;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Reset platform velocity when leaving any collision
        if (collision.collider.GetComponentInParent<OscillatePlatform>() != null)
        {
            platformVelocity = Vector2.zero;
        }
    }

    void UpdateMovementAnimations(float moveInput)
    {
        // Update speed parameter for running animation
        animator.SetFloat("speed", Mathf.Abs(moveInput * moveSpeed));

        // Handle turning
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
        // Flip the character's facing direction
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Flip the character
        transform.localScale = localScale;
    }

    public void UnlockJump()
    {
        canJump = true; // Unlock jumping
        isGrounded = true; // Assume grounded initially
    }

    public void EnableDoubleJump()
    {
        canDoubleJump = true; // Unlock double jump
    }

    public void EnableTripleJump()
    {
        canTripleJump = true;       // Allow triple jump
        tripleJumpAvailable = true; // Enable triple jump power-up
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
            currentLadder = other; // Keep track of the ladder
            rb.gravityScale = 0; // Disable gravity while on the ladder
            rb.linearVelocity = Vector2.zero; // Reset velocity

            // Reset jump states
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
            rb.gravityScale = 1; // Restore gravity
        }
    }
}