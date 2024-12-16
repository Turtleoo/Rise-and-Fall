using UnityEngine;
using System.Collections;

public class ConditionalObjectMover : MonoBehaviour
{
    [Header("References")]
    public Transform objectToMove; // The object to move
    public Lever lever; // Reference to the Lever script
    public ChestInteraction chestInteraction; // Reference to the ChestInteraction script

    [Header("Movement Settings")]
    public float moveDistance = 10f; // Distance to move the object upwards
    public float moveSpeed = 2f; // Speed of the object movement

    [Header("Prompt Settings")]
    public GameObject prompt; // UI prompt to display "Press E"

    [Header("Audio Settings")]
    public AudioSource objectMoveAudioSource; // Optional audio source for moving the object

    private bool playerInRange = false; // Whether the player is within interaction range
    private bool hasMoved = false; // Whether the object has already moved
    private bool chestOpened = false; // Whether the chest has been opened
    private bool leverInteractedAfterChestOpened = false; // Whether the lever has been interacted with after the chest was opened
    private Vector3 startPosition; // Starting position of the object
    private Vector3 targetPosition; // Target position of the object

    void Start()
    {
        // Calculate the target position based on the move distance
        startPosition = objectToMove.position;
        targetPosition = startPosition + new Vector3(0, moveDistance, 0);

        // Ensure the prompt is initially hidden
        if (prompt != null)
        {
            prompt.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the chest has been opened
        if (!chestOpened && chestInteraction != null && chestInteraction.IsChestOpened())
        {
            chestOpened = true;
            Debug.Log("Chest has been opened. Lever interaction condition is now active.");
        }

        // Check if the lever has been interacted with, but only count it if the chest has already been opened
        if (chestOpened && !leverInteractedAfterChestOpened && lever != null && lever.HasLeverBeenInteracted)
        {
            leverInteractedAfterChestOpened = true;
            Debug.Log("Lever has been interacted with after the chest was opened.");
        }

        // Show the prompt if the conditions are met
        if (playerInRange && !hasMoved && chestOpened && leverInteractedAfterChestOpened && prompt != null)
        {
            prompt.SetActive(true);
        }

        // Allow interaction if the player is in range and presses E
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !hasMoved && chestOpened && leverInteractedAfterChestOpened)
        {
            StartCoroutine(MoveObject());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Show the prompt only if all conditions are met and the object hasn't moved yet
            if (chestOpened && leverInteractedAfterChestOpened && !hasMoved && prompt != null)
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

            // Hide the prompt when the player leaves the range
            if (prompt != null)
            {
                prompt.SetActive(false);
            }
        }
    }

    private IEnumerator MoveObject()
    {
        hasMoved = true; // Mark the object as moved

        // Hide the prompt since the interaction is complete
        if (prompt != null)
        {
            prompt.SetActive(false);
        }

        // Play audio if provided
        if (objectMoveAudioSource != null)
        {
            objectMoveAudioSource.Play();
        }

        // Move the object to the target position
        while (Vector3.Distance(objectToMove.position, targetPosition) > 0.01f)
        {
            objectToMove.position = Vector3.MoveTowards(objectToMove.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Stop audio if it's still playing
        if (objectMoveAudioSource != null && objectMoveAudioSource.isPlaying)
        {
            objectMoveAudioSource.Stop();
        }

        Debug.Log("Object movement complete.");
    }
}
