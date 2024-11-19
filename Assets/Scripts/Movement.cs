using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float movement;
    public float moveSpeed = 5f;
    public float jumpForce = 10f; // How high the player jumps
    private bool isGrounded; // Check if the player is on the ground
    private Rigidbody2D rb; // Rigidbody2D for physics-based movement

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get horizontal movement input
        movement = Input.GetAxis("Horizontal");

        // Jump when on the ground and the Jump button is pressed
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false; // Set to false until grounded again
        }
    }

    private void FixedUpdate()
    {
        if (movement != 0)
        {
            // Move the player horizontally
            rb.linearVelocity = new Vector2(movement * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Stop horizontal movement when no input is given
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is touching the ground
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
