using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Prompt Settings")]
    public GameObject prompt;

    [Header("Gear Settings")]
    public Transform gear;
    public float gearRotationSpeed = 90f;

    [Header("Platform Settings")]
    public Transform platform;
    public float platformMoveSpeed = 5f;
    public float moveDuration = 2f;
    public float platformMoveDistance = 10f;

    [Header("Object Movement Settings")]
    public Transform objectToMove;
    public float moveDistance = 20f;
    public float objectMoveSpeed = 2f;

    [Header("Lever Animation")]
    public Animator leverAnimator;

    [Header("Audio Settings")]
    public AudioSource leverFlickAudioSource;
    public AudioSource platformFinishAudioSource;
    public AudioSource objectMoveAudioSource;

    private bool playerInRange = false;
    private static bool isLeverActive = false; // Static variable
    private float moveTimer = 0f;
    private bool hasPlayedPlatformAudio = false;
    private bool leverInteracted = false; // Tracks if the lever has been used
    private bool isObjectMoving = false; // Tracks if the object is currently moving

    private Vector3 platformStartPosition;
    private Vector3 platformTargetPosition;
    private Vector3 objectStartPosition;
    private Vector3 objectTargetPosition;

    public static bool IsLeverActive => isLeverActive;

    private void Awake()
    {
        // Reset static variables to initial state
        isLeverActive = false;
    }

    private void Start()
    {
        if (prompt != null)
        {
            prompt.SetActive(false);
        }

        if (platform != null)
        {
            platformStartPosition = platform.position;
            platformTargetPosition = platformStartPosition + new Vector3(platformMoveDistance, 0, 0);
        }

        if (objectToMove != null)
        {
            objectStartPosition = objectToMove.position;
            objectTargetPosition = objectStartPosition + new Vector3(0, moveDistance, 0);
        }

        ResetPositions();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleLever();
        }

        if (moveTimer > 0)
        {
            moveTimer -= Time.deltaTime;

            if (platform != null)
            {
                Vector3 targetPosition = isLeverActive ? platformTargetPosition : platformStartPosition;
                platform.position = Vector3.MoveTowards(platform.position, targetPosition, platformMoveSpeed * Time.deltaTime);
            }

            if (gear != null)
            {
                float rotationDirection = isLeverActive ? 1 : -1;
                gear.Rotate(0, 0, rotationDirection * gearRotationSpeed * Time.deltaTime);
            }

            hasPlayedPlatformAudio = false;
        }
        else if (!hasPlayedPlatformAudio && leverInteracted)
        {
            if (platformFinishAudioSource != null)
            {
                platformFinishAudioSource.Play();
            }
            hasPlayedPlatformAudio = true;
        }

        if (objectToMove != null)
        {
            Vector3 targetPosition = isLeverActive ? objectTargetPosition : objectStartPosition;
            if (Vector3.Distance(objectToMove.position, targetPosition) > 0.01f)
            {
                objectToMove.position = Vector3.MoveTowards(objectToMove.position, targetPosition, objectMoveSpeed * Time.deltaTime);
                if (!isObjectMoving)
                {
                    // Start playing object move audio
                    if (objectMoveAudioSource != null && !objectMoveAudioSource.isPlaying)
                    {
                        objectMoveAudioSource.Play();
                    }
                    isObjectMoving = true;
                }
            }
            else
            {
                // Stop playing object move audio
                if (isObjectMoving && objectMoveAudioSource != null && objectMoveAudioSource.isPlaying)
                {
                    objectMoveAudioSource.Stop();
                }
                isObjectMoving = false;
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
                prompt.SetActive(false);
            }
        }
    }

    private void ToggleLever()
    {
        isLeverActive = !isLeverActive;
        leverInteracted = true;

        if (leverFlickAudioSource != null)
        {
            leverFlickAudioSource.Play();
        }

        moveTimer = moveDuration;

        if (leverAnimator != null)
        {
            leverAnimator.SetBool("IsActive", isLeverActive);
        }

        if (prompt != null)
        {
            prompt.SetActive(false);
        }
    }

    private void ResetPositions()
    {
        if (platform != null)
        {
            platform.position = platformStartPosition;
        }

        if (objectToMove != null)
        {
            objectToMove.position = objectStartPosition;
        }
    }
}
