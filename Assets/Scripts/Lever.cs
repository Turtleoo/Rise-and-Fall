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

    [Header("Object Movement Settings")]
    public Transform objectToMove; // The object to move up and down
    public float moveDistance = 3f; // The distance to move the object
    public float objectMoveSpeed = 2f; // Speed of the object's movement

    [Header("Lever Animation")]
    public Animator leverAnimator; // Reference to the Animator for the lever

    [Header("Audio Settings")]
    public AudioSource leverFlickAudioSource; // Audio for when the lever is flicked
    public AudioSource platformFinishAudioSource; // Audio for when the platform finishes moving
    public AudioSource objectMoveAudioSource; // Looping audio for while the object is moving

    private bool playerInRange = false; // Tracks if the player is near the lever
    private bool isLeverActive = false; // Tracks the state of the lever
    private float moveTimer = 0f; // Tracks how long the platform has been moving
    private bool hasPlayedPlatformAudio = false; // Tracks if the platform audio has been played after movement
    private bool isPlatformMovingInitially = false; // Tracks whether the platform has started moving since the game started

    private Vector3 objectStartPosition; // Stores the original position of the object
    private Vector3 objectTargetPosition; // Stores the target position for the object

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

        if (objectToMove != null)
        {
            objectStartPosition = objectToMove.position; // Save the object's start position
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

            hasPlayedPlatformAudio = false; // Reset the audio flag while moving
        }
        else if (!hasPlayedPlatformAudio && isPlatformMovingInitially)
        {
            // Play platform audio after it finishes moving
            if (platformFinishAudioSource != null)
            {
                platformFinishAudioSource.Play();
            }
            hasPlayedPlatformAudio = true;
        }

        // Move the objectToMove up or down
        if (objectToMove != null)
        {
            Vector3 targetPosition = isLeverActive ? objectTargetPosition : objectStartPosition;
            if (objectToMove.position != targetPosition)
            {
                if (objectMoveAudioSource != null && !objectMoveAudioSource.isPlaying)
                {
                    objectMoveAudioSource.Play(); // Play object moving audio
                }

                objectToMove.position = Vector3.MoveTowards(objectToMove.position, targetPosition, objectMoveSpeed * Time.deltaTime);
            }
            else
            {
                if (objectMoveAudioSource != null && objectMoveAudioSource.isPlaying)
                {
                    objectMoveAudioSource.Stop(); // Stop object moving audio
                }
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

        // Set the flag to indicate the platform has started moving
        isPlatformMovingInitially = true;

        // Play lever flick audio
        if (leverFlickAudioSource != null)
        {
            leverFlickAudioSource.Play();
        }

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

        // Calculate the new target position for the object
        if (objectToMove != null)
        {
            objectTargetPosition = objectStartPosition + new Vector3(0, moveDistance, 0);
        }

        // Hide the prompt after interacting
        if (prompt != null)
        {
            prompt.SetActive(false);
        }

        Debug.Log("Lever toggled. State: " + (isLeverActive ? "Active" : "Inactive"));
    }
}
