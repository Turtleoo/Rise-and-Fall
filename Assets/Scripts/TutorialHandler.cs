using UnityEngine;
using System.Collections;

public class TutorialHandler : MonoBehaviour
{
    [Header("Tutorial Panel Settings")]
    public GameObject tutorialPanel; // Parent object containing all child prompts
    public float textInterval = 2.0f; // Time interval between each prompt display
    public string playerTag = "Player"; // Tag of the player object

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
        foreach (Transform prompt in tutorialPrompts)
        {
            prompt.gameObject.SetActive(true); // Show the current prompt
            Debug.Log($"Displaying prompt: {prompt.name}");
            yield return new WaitForSeconds(textInterval); // Wait for the interval
            prompt.gameObject.SetActive(false); // Hide the current prompt
        }

        // Deactivate the tutorial panel after the sequence
        tutorialPanel.SetActive(false);
        Debug.Log("Tutorial ended.");
    }
}
