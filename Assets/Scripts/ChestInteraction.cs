using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class ChestInteraction : MonoBehaviour
{
    [Header("UI and Player References")]
    public GameObject prompt; // UI element to display "Press E"
    public Movement playerMovement; // Reference to the Movement script
    public GameObject escapeTutorial; // Reference to the EscapeTutorial GameObject (the parent Canvas)

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
    public float textInterval = 2.0f; // Time interval between each text display

    private bool playerInRange = false;
    private bool chestOpened = false; // Tracks whether the chest has already been opened
    private Transform[] tutorialTexts; // Array to store child text prompts

    void Start()
    {
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
            else
            {
                Debug.LogError("Chest open sound AudioSource is not assigned!");
            }

            // Trigger the chest opening animation
            if (chestAnimator != null)
            {
                chestAnimator.SetBool("IsOpen", true);
                Debug.Log("Chest animation triggered! Animator 'IsOpen' set to true.");
            }
            else
            {
                Debug.LogError("Chest Animator is not assigned!");
            }

            // Start the escape sequence tutorial
            StartCoroutine(PlayEscapeTutorial());
        }
    }

    IEnumerator PlayEscapeTutorial()
    {
        // Activate the EscapeTutorial parent GameObject
        if (escapeTutorial != null)
        {
            escapeTutorial.SetActive(true);
            Debug.Log("Escape tutorial started.");
        }

        // Iterate through all tutorial texts
        foreach (Transform text in tutorialTexts)
        {
            if (text != escapeTutorial.transform) // Skip the parent itself
            {
                text.gameObject.SetActive(true); // Show the text
                Debug.Log($"Displaying tutorial text: {text.name}");
                yield return new WaitForSeconds(textInterval); // Wait for the interval
                text.gameObject.SetActive(false); // Hide the text
            }
        }

        // Deactivate the EscapeTutorial parent after displaying all texts
        if (escapeTutorial != null)
        {
            escapeTutorial.SetActive(false);
            Debug.Log("Escape tutorial ended.");
        }

        // Unlock the player's ability to jump
        if (playerMovement != null)
        {
            playerMovement.UnlockJump(); // Unlock the ability to jump
            playerMovement.DisableGlide(); // Disable gliding functionality
            Debug.Log("Player jump unlocked and gliding disabled.");
        }

        // Trigger the barrel animation
        if (barrelAnimator != null)
        {
            barrelAnimator.SetTrigger("PlayBarrelAnimation");
            Debug.Log("Barrel animation triggered.");
        }
        else
        {
            Debug.LogError("Barrel Animator is not assigned!");
        }

        // Wait for a brief moment to ensure the animation starts
        yield return new WaitForSeconds(0.5f);

        // Play the music after the barrel breaks
        if (barrelBreakMusic != null)
        {
            barrelBreakMusic.Play();
            Debug.Log("Barrel break music played.");
        }
        else
        {
            Debug.LogError("Barrel break music AudioSource is not assigned!");
        }

        // Play additional music
        if (additionalMusic != null)
        {
            additionalMusic.Play();
            Debug.Log("Additional music played.");
        }
        else
        {
            Debug.LogError("Additional music AudioSource is not assigned!");
        }

        // Mute specified audio groups
        if (audioMixer != null && audioGroupsToMute.Length > 0)
        {
            foreach (string group in audioGroupsToMute)
            {
                audioMixer.SetFloat(group, -80f); // Set the volume to a very low level (effectively mute)
                Debug.Log($"Muted audio group: {group}");
            }
        }
    }
}