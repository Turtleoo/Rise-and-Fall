using UnityEngine;

public class JumpControlTrigger : MonoBehaviour
{
    [Header("Jump Settings")]
    public bool disableJump = true; // Set to true for the start trigger, false for the end trigger

    [Header("Platform Settings")]
    public Transform platform;
    public float platformMoveSpeed = 5f;
    public float moveDuration = 2f;
    public float platformMoveDistance = -10f; // Negative for left movement

    [Header("Gear Settings")]
    public Transform gear;
    public float gearRotationSpeed = -90f; // Negative for left rotation
    public float gearRotationDuration = 2f; // Time in seconds for gear rotation

    [Header("Audio Settings")]
    public AudioSource platformMoveAudioSource;
    public AudioSource gearRotateAudioSource;

    private Vector3 platformStartPosition;
    private Vector3 platformTargetPosition;
    private float platformMoveTimer = 0f;
    private float gearRotationTimer = 0f;
    private bool isMovingPlatform = false;

    private bool hasTriggered = false;

    private void Start()
    {
        // Initialize platform positions
        if (platform != null)
        {
            platformStartPosition = platform.position;
            platformTargetPosition = platformStartPosition + new Vector3(platformMoveDistance, 0, 0);
        }
    }

    private void Update()
    {
        // Handle platform movement
        if (platformMoveTimer > 0)
        {
            platformMoveTimer -= Time.deltaTime;

            if (platform != null)
            {
                platform.position = Vector3.MoveTowards(platform.position, platformTargetPosition, platformMoveSpeed * Time.deltaTime);

                if (!isMovingPlatform && platformMoveAudioSource != null)
                {
                    platformMoveAudioSource.Play();
                    isMovingPlatform = true;
                }
            }
        }
        else
        {
            if (isMovingPlatform && platformMoveAudioSource != null)
            {
                platformMoveAudioSource.Stop();
                isMovingPlatform = false;
            }
        }

        // Handle gear rotation
        if (gearRotationTimer > 0)
        {
            gearRotationTimer -= Time.deltaTime;

            if (gear != null)
            {
                gear.Rotate(0, 0, gearRotationSpeed * Time.deltaTime);

                if (gearRotateAudioSource != null && !gearRotateAudioSource.isPlaying)
                {
                    gearRotateAudioSource.Play();
                }
            }
        }
        else
        {
            if (gearRotateAudioSource != null && gearRotateAudioSource.isPlaying)
            {
                gearRotateAudioSource.Stop();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // Mark as triggered

            // Handle jump enable/disable
            Movement playerMovement = other.GetComponent<Movement>();

            if (playerMovement != null)
            {
                if (disableJump)
                {
                    playerMovement.DisableJump(); // Disable jump
                }
                else
                {
                    playerMovement.EnableJump(); // Enable jump
                }
            }

            // Start platform movement
            platformMoveTimer = moveDuration;

            // Start gear rotation
            gearRotationTimer = gearRotationDuration;

            if (platform != null)
            {
                platformTargetPosition = platformStartPosition + new Vector3(platformMoveDistance, 0, 0);
            }
        }
    }
}
