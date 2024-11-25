using System.Linq;
using UnityEngine;
using System.Collections;

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

    // Audio sources for jump and glide
    public AudioSource jumpAudioSource;
    public AudioSource glideAudioSource;

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

        // Jump logic
        if (canJump && Input.GetButtonDown("Jump"))
        {
            if (isGrounded || isOnLadder)
            {
                Jump();
                hasDoubleJumped = false;
                hasTripleJumped = false;

                if (isOnLadder)
                {
                    isOnLadder = false;
                    currentLadder = null;
                    rb.gravityScale = 1;
                }
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

        // Glide logic: Always active when falling and gliding is enabled
        if (canGlide && !isGrounded && rb.linearVelocity.y < 0)
        {
            if (Input.GetKey(KeyCode.S)) // Fast-fall input
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fastFallSpeed);

                if (!isFastFalling)
                {
                    isFastFalling = true;
                    animator.SetBool("IsFastFalling", true);
                }

                if (isGliding)
                {
                    isGliding = false;

                    // Stop gliding audio
                    if (glideAudioSource != null && glideAudioSource.isPlaying)
                    {
                        glideAudioSource.Stop();
                    }

                    animator.SetBool("IsFlying", false);
                }
            }
            else
            {
                if (!isGliding)
                {
                    isGliding = true;

                    // Play gliding audio
                    if (glideAudioSource != null && !glideAudioSource.isPlaying)
                    {
                        glideAudioSource.Play();
                    }
                }

                isFastFalling = false;
                animator.SetBool("IsFastFalling", false);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -glideFallSpeed);

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
                isGliding = false;

                // Stop gliding audio
                if (glideAudioSource != null && glideAudioSource.isPlaying)
                {
                    glideAudioSource.Stop();
                }

                animator.SetBool("IsFlying", false);
            }

            isFastFalling = false;
            animator.SetBool("IsFastFalling", false);
        }

        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    public void DisableGlideTemporarily()
    {
        canGlide = false;
        animator.SetBool("IsFlying", false); // Stop flying animation if gliding is disabled
        animator.SetBool("IsFastFalling", false); // Stop fast-falling animation if gliding is disabled
        Debug.Log("Glide temporarily disabled.");
    }

    private IEnumerator ReEnableGlideAfterAnimation(string animationName)
    {
        if (animator != null)
        {
            // Wait for the specified animation to finish
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f)
            {
                yield return null; // Wait for the next frame
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            // Re-enable gliding
            canGlide = true;
            Debug.Log("Glide re-enabled after animation.");
        }
    }

    public void HandleHit()
    {
        if (animator != null)
        {
            DisableGlideTemporarily();
            animator.SetTrigger("Hit"); // Play hit animation
            StartCoroutine(ReEnableGlideAfterAnimation("Hit")); // Re-enable gliding after the hit animation
        }
    }

    public void HandleDeath()
    {
        if (animator != null)
        {
            DisableGlideTemporarily();
            animator.SetTrigger("Die"); // Play death animation
            StartCoroutine(ReEnableGlideAfterAnimation("Die")); // Re-enable gliding after the death animation
        }

        // Additional death logic can go here
        Debug.Log("Player has died.");
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

        if (jumpAudioSource != null)
        {
            jumpAudioSource.Play();
        }
    }

    void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.2f);
        hasDoubleJumped = true;
        animator.SetBool("IsDoubleJump", true);
        isGrounded = false;

        if (jumpAudioSource != null)
        {
            jumpAudioSource.Play();
        }
    }

    void TripleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.5f);
        hasTripleJumped = true;
        tripleJumpAvailable = false;
        animator.SetBool("IsJump", true);
        isGrounded = false;

        if (jumpAudioSource != null)
        {
            jumpAudioSource.Play();
        }
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
