using UnityEngine;
using System.Collections;

public class TutorialHandler : MonoBehaviour
{
    [Header("Tutorial Panel Settings")]
    public GameObject tutorialPanel; // Parent object containing all child prompts
    public float textInterval = 2.0f; // Time interval between each prompt display
    public string playerTag = "Player"; // Tag of the player object

    [Header("Lever Reference")]
    public Lever lever; // Reference to the Lever script

    [Header("Camera Pan Settings")]
    public FollowCamera followCamera; // Reference to the FollowCamera script
    public Transform[] cameraTargets; // Targets to pan to for each tutorial prompt
    public int[] panPrompts; // Indices of prompts where the camera should pan
    public float panDuration = 2f; // Duration of camera panning for each target

    private Transform[] tutorialPrompts; // Array of child prompts
    private bool tutorialStarted = false; // Prevents retriggering the tutorial

    private void Start()
    {
        // Ensure the tutorial panel is initially disabled
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);

            // Get all child objects except the parent itself
            tutorialPrompts = tutorialPanel.GetComponentsInChildren<Transform>(true);
            tutorialPrompts = System.Array.FindAll(tutorialPrompts, prompt => prompt != tutorialPanel.transform);

            if (tutorialPrompts.Length == 0)
            {
                Debug.LogError("No tutorial prompts found in the panel!");
            }
        }
        else
        {
            Debug.LogError("Tutorial panel is not assigned!");
        }

        // Ensure lever reference is assigned
        if (lever == null)
        {
            Debug.LogError("Lever reference is not assigned!");
        }

        // Ensure followCamera and cameraTargets are properly set
        if (followCamera == null)
        {
            Debug.LogError("FollowCamera reference is not assigned!");
        }
        if (cameraTargets == null || cameraTargets.Length == 0)
        {
            Debug.LogError("Camera targets are not assigned!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag(playerTag) && !tutorialStarted)
        {
            Debug.Log("Player entered tutorial trigger.");
            tutorialStarted = true; // Prevent retriggering
            StartCoroutine(PlayTutorialSequence());
        }
    }

    private IEnumerator PlayTutorialSequence()
    {
        // Ensure tutorial prompts are set up correctly
        if (tutorialPrompts == null || tutorialPrompts.Length == 0)
        {
            Debug.LogError("Tutorial prompts are not set up correctly.");
            yield break;
        }

        // Activate the tutorial panel
        tutorialPanel.SetActive(true);
        Debug.Log("Tutorial started.");

        // Display each child prompt in sequence
        for (int i = 0; i < tutorialPrompts.Length; i++)
        {
            // Check if the current prompt requires camera panning
            if (System.Array.Exists(panPrompts, index => index == i))
            {
                int targetIndex = System.Array.IndexOf(panPrompts, i);
                if (targetIndex >= 0 && targetIndex < cameraTargets.Length && followCamera != null)
                {
                    yield return StartCoroutine(PanToTarget(cameraTargets[targetIndex]));
                }
            }

            tutorialPrompts[i].gameObject.SetActive(true); // Show the current prompt
            Debug.Log($"Displaying prompt: {tutorialPrompts[i].name}");
            yield return new WaitForSeconds(textInterval); // Wait for the interval
            tutorialPrompts[i].gameObject.SetActive(false); // Hide the current prompt
        }

        // Deactivate the tutorial panel after the sequence
        tutorialPanel.SetActive(false);
        Debug.Log("Tutorial ended.");

        // Notify the lever that the tutorial is complete
        if (lever != null)
        {
            lever.CompleteTutorial();
            Debug.Log("Lever unlocked.");
        }
    }

    private IEnumerator PanToTarget(Transform target)
    {
        if (followCamera != null && target != null)
        {
            // Temporarily pan the camera to the target
            followCamera.StartCameraPanToTarget(target.position, panDuration);
            yield return new WaitForSeconds(panDuration + followCamera.focusDuration); // Wait for pan and focus duration
        }
    }
}
