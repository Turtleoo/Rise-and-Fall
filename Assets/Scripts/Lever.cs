using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Prompt Settings")]
    public GameObject prompt; // UI element to display "Press E"

    [Header("Gear Settings")]
    public Transform gear; // Reference to the gear
    public float gearRotationSpeed = 90f; // Degrees per second

    [Header("Platform Settings")]
    public Transform platform; // Reference to the platform
    public float platformMoveSpeed = 5f; // Units per second
    public float moveDuration = 2f; // Duration for the platform to move in seconds

    [Header("Lever Animation")]
    public Animator leverAnimator; // Reference to the Animator for the lever

    private bool playerInRange = false; // Tracks if the player is near the lever
    private bool isLeverActive = false; // Tracks the state of the lever
    private float moveTimer = 0f; // Tracks how long the platform has been moving

    private void Start()
    {
        if (prompt != null)
        {
            prompt.SetActive(false); // Ensure the prompt is hidden initially
        }

        if (leverAnimator == null)
        {
            Debug.LogError("leverAnimator is not assigned! Please assign it in the Inspector.");
        }
    }

    private void Update()
    {
        // Check for interaction when the player is in range and presses E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleLever();
        }

        // Handle platform and gear movement while the timer is active
        if (moveTimer > 0)
        {
            moveTimer -= Time.deltaTime;

            // Move the platform
            if (platform != null)
            {
                float direction = isLeverActive ? 1 : -1; // Move right if active, left otherwise
                platform.position += new Vector3(direction * platformMoveSpeed * Time.deltaTime, 0, 0);
            }

            // Rotate the gear
            if (gear != null)
            {
                float rotationDirection = isLeverActive ? 1 : -1; // Rotate right if active, left otherwise
                gear.Rotate(0, 0, rotationDirection * gearRotationSpeed * Time.deltaTime);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (prompt != null)
            {
                prompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (prompt != null)
            {
                prompt.SetActive(false); // Hide the prompt
            }
        }
    }

    private void ToggleLever()
    {
        // Toggle the lever state
        isLeverActive = !isLeverActive;

        // Reset the move timer
        moveTimer = moveDuration;

        // Trigger the lever animation
        if (leverAnimator != null)
        {
            leverAnimator.SetBool("IsActive", isLeverActive);
            Debug.Log($"SetBool called: IsActive = {isLeverActive}");
        }
        else
        {
            Debug.LogError("leverAnimator is not assigned.");
        }

        // Hide the prompt after interacting
        if (prompt != null)
        {
            prompt.SetActive(false);
        }

        Debug.Log("Lever toggled. State: " + (isLeverActive ? "Active" : "Inactive"));
    }
}
