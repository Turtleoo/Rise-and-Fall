using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    public float moveSpeed = 5f; // Speed for horizontal movement
    public float jumpForce = 1f; // Force applied for jumping
    private bool canJump = false; // Initially, the player cannot jump
    private bool canDoubleJump = false; // Can the player double jump?
    private bool hasDoubleJumped = false; // Has the double jump been used?
    private bool canTripleJump = false; // Can the player triple jump?
    private bool hasTripleJumped = false; // Has the triple jump been used?
    private bool tripleJumpAvailable = false; // Is triple jump power-up active?
    private Rigidbody2D rb; // Reference to Rigidbody2D
    private bool isGrounded; // Is the player on the ground?
    private bool isFacingRight = true; // Tracks the player's facing direction

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing. Please add it to the GameObject.");
        }
    }

    void Update()
    {
        // Handle horizontal movement
        float moveInput = Input.GetAxis("Horizontal");

        // Apply movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Update the speed animation parameter with a threshold
        if (Mathf.Abs(moveInput) > 0.1f) // Avoid tiny input values
        {
            animator.SetFloat("speed", Mathf.Abs(moveInput)); // Set speed based on input
        }
        else
        {
            animator.SetFloat("speed", 0f); // Set speed to 0 when no movement
        }

        // Handle jumping logic
        if (canJump && Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump(); // Normal jump
                animator.SetBool("IsJump", true); // Trigger jumping animation
                ResetJumpState(); // Reset double/triple jump states
            }
            else if (canTripleJump && tripleJumpAvailable && !hasTripleJumped)
            {
                TripleJump();
            }
            else if (canDoubleJump && !hasDoubleJumped)
            {
                DoubleJump();
            }
        }

        // Handle turning animation
        HandleTurningAnimation(moveInput);

        // Handle falling animation
        HandleFallingAnimation();
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false; // Set to false after jumping
        animator.SetBool("IsJump", true); // Trigger jump animation
    }

    void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.2f);
        hasDoubleJumped = true;
        animator.SetTrigger("DoubleJump"); // Trigger double jump animation
    }

    void TripleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.5f);
        hasTripleJumped = true;
        tripleJumpAvailable = false; // Expend the triple jump power-up
        animator.SetTrigger("TripleJump"); // Trigger triple jump animation
        Debug.Log("Triple jump performed!");
    }

    void HandleTurningAnimation(float moveInput)
    {
        if (moveInput < 0 && isFacingRight)
        {
            Flip(); // Flip the sprite to face left
            animator.SetBool("IsTurn", true);
        }
        else if (moveInput > 0 && !isFacingRight)
        {
            Flip(); // Flip the sprite to face right
            animator.SetBool("IsTurn", true);
        }
        else
        {
            animator.SetBool("IsTurn", false); // Stop turning animation
        }
    }

    void HandleFallingAnimation()
    {
        // Only play falling animation when not grounded and moving downward
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            animator.SetBool("IsFalling", true); // Activate falling animation
        }
        else
        {
            animator.SetBool("IsFalling", false); // Deactivate falling animation
        }
    }

    void Flip()
    {
        // Flip the player's facing direction
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Flip the sprite on the x-axis
        transform.localScale = localScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Any(contact => contact.normal.y > 0.5f)) // Ensure contact from below
        {
            isGrounded = true;

            // Only trigger landing if falling animation was playing
            if (animator.GetBool("IsFalling"))
            {
                animator.SetTrigger("IsLanding"); // Trigger landing animation
            }

            animator.SetBool("IsJump", false); // Stop jumping animation
            animator.SetBool("IsFalling", false); // Stop falling animation
            ResetJumpState();
        }
    }

    void ResetJumpState()
    {
        hasDoubleJumped = false; // Reset double jump state
        hasTripleJumped = false; // Reset triple jump state
    }

    public void UnlockJump()
    {
        canJump = true;
        isGrounded = true;
    }

    public void EnableDoubleJump()
    {
        canDoubleJump = true; // Unlock double jump
    }

    public void EnableTripleJump()
    {
        canTripleJump = true;       // Allow triple jump
        tripleJumpAvailable = true; // Triple jump power-up is active
    }
}
