using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class ChestInteraction : MonoBehaviour
{
    [Header("UI and Player References")]
    public GameObject prompt; // UI element to display "Press E"
    public Movement playerMovement; // Reference to the Movement script
    public GameObject escapeTutorial; // Reference to the EscapeTutorial GameObject (the parent Canvas)
    public Transform cameraPanTarget; // Target object for camera panning
    public float cameraPanDuration = 2f; // Duration of the camera pan
    public float cameraReturnDuration = 2f; // Duration for the camera to return to the player

    [Header("Chest Interaction")]
    public Animator chestAnimator; // Animator for the chest
    public AudioSource chestOpenSound; // AudioSource for the chest opening sound

    [Header("Barrel Interaction")]
    public Animator barrelAnimator; // Reference to the barrel's Animator component
    public AudioSource barrelBreakMusic; // AudioSource for the music after the barrel breaks
    public AudioSource additionalMusic; // Additional AudioSource to play after the barrel breaks
    public AudioMixer audioMixer; // Reference to the AudioMixer
    public string[] audioGroupsToMute; // Array of exposed parameters in the AudioMixer to mute

    [Header("Escape Tutorial Settings")]
    public float[] textIntervals; // Array of intervals for each text display

    [Header("Object Toggle Settings")]
    public GameObject[] objectsToDisable; // Array of objects to disable
    public GameObject[] objectsToEnable; // Array of objects to enable

    [Header("Platform Settings")]
    public Transform platform; // Reference to the platform
    public float platformMoveSpeed = 5f; // Speed of platform movement
    public float platformMoveDistance = -4.6f; // Distance to move the platform
    public float platformMoveDelay = 2f; // Delay before moving the platform
    public float platformMoveDuration = 2f; // Duration to move the platform
    public AudioSource platformMoveAudio; // AudioSource for platform movement

    [Header("Gear Settings")]
    public Transform gear; // Reference to the gear
    public float gearRotationSpeed = 180f; // Speed of gear rotation
    public float gearRotationDuration = 2f; // Duration of gear rotation
    public float gearRotationDelay = 2f; // Delay before rotating the gear
    public AudioSource gearRotationAudio; // AudioSource for gear rotation

    [Header("Player Settings")]
    public float newFastFallSpeed = 15f; // New fast fall speed to apply when the chest is opened

    [Header("Key Object Settings")]
    public Transform keyObject; // Reference to the key object
    public float keyMoveSpeed = 2f; // Speed of the key's upward movement
    public float keyMoveDuration = 2f; // Duration the key moves upwards
    public float keyHideDelay = 1f; // Delay before the key is hidden or destroyed after movement

    private bool playerInRange = false;
    private bool chestOpened = false; // Tracks whether the chest has already been opened
    private Vector3 platformStartPosition; // Original position of the platform
    private Vector3 platformTargetPosition; // Target position of the platform
    private Transform[] tutorialTexts; // Array to store child text prompts
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Initialize platform positions
        if (platform != null)
        {
            platformStartPosition = platform.position;
            platformTargetPosition = platformStartPosition + new Vector3(platformMoveDistance, 0, 0);
        }

        // Hide the prompt initially
        if (prompt != null) prompt.SetActive(false);

        // Ensure the parent escape tutorial and its children are hidden
        if (escapeTutorial != null)
        {
            escapeTutorial.SetActive(false);
            tutorialTexts = escapeTutorial.GetComponentsInChildren<Transform>(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !chestOpened) // Only show prompt if chest is unopened
        {
            playerInRange = true;
            if (prompt != null) prompt.SetActive(true);

            // Automatically get the Movement component
            if (playerMovement == null)
            {
                playerMovement = other.GetComponent<Movement>();
            }

            Debug.Log("Player entered the chest's interaction range.");
        }
    }

    public bool IsChestOpened()
    {
        return chestOpened;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (prompt != null) prompt.SetActive(false);

            Debug.Log("Player exited the chest's interaction range.");
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !chestOpened)
        {
            chestOpened = true; // Mark the chest as opened
            Debug.Log("Chest opened for the first time.");

            // Hide the prompt permanently
            if (prompt != null)
            {
                prompt.SetActive(false);
                Debug.Log("Prompt has been hidden.");
            }

            // Play the chest opening sound
            if (chestOpenSound != null)
            {
                chestOpenSound.Play();
                Debug.Log("Chest opening sound played.");
            }

            // Trigger the chest opening animation
            if (chestAnimator != null)
            {
                chestAnimator.SetBool("IsOpen", true);
                Debug.Log("Chest animation triggered!");
            }

            // Disable gliding and update fast fall speed
            if (playerMovement != null)
            {
                playerMovement.DisableGlide();
                playerMovement.fastFallSpeed = newFastFallSpeed;
            }

            // Disable and enable specified objects
            ToggleGameObjects();

            // Start delayed platform and gear actions
            StartCoroutine(DelayedPlatformMovement());
            StartCoroutine(DelayedGearRotation());

            // Start key object movement and play escape tutorial afterward
            if (keyObject != null)
            {
                StartCoroutine(KeyObjectMovementThenTutorial());
            }
        }
    }

    private void ToggleGameObjects()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null) obj.SetActive(false);
        }
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null) obj.SetActive(true);
        }
    }

    IEnumerator KeyObjectMovementThenTutorial()
    {
        float elapsedTime = 0f;

        // Move the key object upwards
        while (elapsedTime < keyMoveDuration)
        {
            keyObject.position += new Vector3(0, keyMoveSpeed * Time.deltaTime, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Wait before hiding or destroying the key
        yield return new WaitForSeconds(keyHideDelay);
        Destroy(keyObject.gameObject);

        // After the key is destroyed, play the escape tutorial
        yield return StartCoroutine(PlayEscapeTutorial());
    }

    IEnumerator DelayedPlatformMovement()
    {
        yield return new WaitForSeconds(platformMoveDelay);

        float elapsedTime = 0f;
        while (elapsedTime < platformMoveDuration)
        {
            platform.position = Vector3.MoveTowards(platform.position, platformTargetPosition, platformMoveSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (platformMoveAudio != null)
        {
            platformMoveAudio.Play();
        }
    }

    IEnumerator DelayedGearRotation()
    {
        yield return new WaitForSeconds(gearRotationDelay);

        float timer = gearRotationDuration;
        while (timer > 0)
        {
            gear.Rotate(0, 0, gearRotationSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        if (gearRotationAudio != null)
        {
            gearRotationAudio.Play();
        }
    }

    IEnumerator PlayEscapeTutorial()
    {
        if (escapeTutorial != null)
        {
            escapeTutorial.SetActive(true);
        }

        for (int i = 0; i < tutorialTexts.Length; i++)
        {
            if (i < textIntervals.Length && tutorialTexts[i] != escapeTutorial.transform)
            {
                if (i == 3)
                {
                    yield return StartCoroutine(TriggerCameraPan());
                }

                tutorialTexts[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(textIntervals[i]);
                tutorialTexts[i].gameObject.SetActive(false);
            }
        }

        if (escapeTutorial != null)
        {
            escapeTutorial.SetActive(false);
        }

        if (barrelAnimator != null)
        {
            barrelAnimator.SetTrigger("PlayBarrelAnimation");
        }
        if (barrelBreakMusic != null)
        {
            barrelBreakMusic.Play();
        }
        if (additionalMusic != null)
        {
            additionalMusic.Play();
        }
    }

    private IEnumerator TriggerCameraPan()
    {
        var followCamera = Camera.main.GetComponent<FollowCamera>();
        if (followCamera != null)
        {
            followCamera.StartCameraPanToTarget(cameraPanTarget.position, cameraPanDuration);
            float panDuration = followCamera.focusDuration + followCamera.panSpeed * 2;
            yield return new WaitForSeconds(panDuration);
        }
    }
}
