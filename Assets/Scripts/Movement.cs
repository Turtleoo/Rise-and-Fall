using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Animator animator;
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Handle horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Jump logic (only if jumping is unlocked)
        if (canJump && Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump(); // Normal jump
                hasDoubleJumped = false; // Reset double jump
                hasTripleJumped = false; // Reset triple jump
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
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false; // Set to false after jumping
    }

    void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.2f);
        hasDoubleJumped = true;
    }

    void TripleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.5f);
        hasTripleJumped = true;
        tripleJumpAvailable = false; // Expend the triple jump power-up
        Debug.Log("Triple jump performed!");
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts.Any(contact => contact.normal.y > 0.5f))
        {
            isGrounded = true;
            hasDoubleJumped = false; // Reset when grounded
            hasTripleJumped = false; // Reset when grounded
        }
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