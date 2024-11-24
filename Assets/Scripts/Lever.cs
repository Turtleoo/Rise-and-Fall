using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Prompt Settings")]
    public GameObject prompt; // UI element to display "Press E"

    [Header("Gear Settings")]
    public Transform gear;
    public float gearRotationSpeed = 90f;

    [Header("Platform Settings")]
    public Transform platform;
    public float platformMoveSpeed = 5f;
    public float moveDuration = 2f;

    [Header("Object Movement Settings")]
    public Transform objectToMove;
    public float moveDistance = 3f;
    public float objectMoveSpeed = 2f;

    [Header("Lever Animation")]
    public Animator leverAnimator;

    [Header("Audio Settings")]
    public AudioSource leverFlickAudioSource;
    public AudioSource platformFinishAudioSource;
    public AudioSource objectMoveAudioSource;

    private bool playerInRange = false;
    private static bool isLeverActive = false;
    private float moveTimer = 0f;
    private bool hasPlayedPlatformAudio = false;

    private Vector3 objectStartPosition;
    private Vector3 objectTargetPosition;

    public static bool IsLeverActive => isLeverActive;

    private void Start()
    {
        if (prompt != null)
        {
            prompt.SetActive(false);
        }

        if (objectToMove != null)
        {
            objectStartPosition = objectToMove.position;
        }
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
                float direction = isLeverActive ? 1 : -1;
                platform.position += new Vector3(direction * platformMoveSpeed * Time.deltaTime, 0, 0);
            }

            if (gear != null)
            {
                float rotationDirection = isLeverActive ? 1 : -1;
                gear.Rotate(0, 0, rotationDirection * gearRotationSpeed * Time.deltaTime);
            }

            hasPlayedPlatformAudio = false;
        }
        else if (!hasPlayedPlatformAudio)
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
            objectToMove.position = Vector3.MoveTowards(objectToMove.position, targetPosition, objectMoveSpeed * Time.deltaTime);
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

        if (leverFlickAudioSource != null)
        {
            leverFlickAudioSource.Play();
        }

        moveTimer = moveDuration;

        if (leverAnimator != null)
        {
            leverAnimator.SetBool("IsActive", isLeverActive);
        }

        if (objectToMove != null)
        {
            objectTargetPosition = objectStartPosition + new Vector3(0, moveDistance, 0);
        }

        if (prompt != null)
        {
            prompt.SetActive(false);
        }
    }
}
