using UnityEngine;

public class VictoryDoor : MonoBehaviour
{
    [Header("Prompt Settings")]
    public GameObject prompt; // UI element to display "Press E"

    [Header("Teleport Settings")]
    public Transform player; // Reference to the player
    public GameObject victoryObject; // Reference to the target VictoryObject

    private bool playerInRange = false; // Tracks if the player is near the door

    private void Start()
    {
        if (prompt != null)
        {
            prompt.SetActive(false); // Ensure the prompt is hidden initially
        }
    }

    private void Update()
    {
        // Check for interaction when the player is in range and presses E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TeleportPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (prompt != null)
            {
                prompt.SetActive(true); // Show the prompt
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

    private void TeleportPlayer()
    {
        if (player != null && victoryObject != null)
        {
            // Teleport the player to the position of the VictoryObject
            player.position = victoryObject.transform.position;

            // Hide the prompt after interacting
            if (prompt != null)
            {
                prompt.SetActive(false);
            }

            Debug.Log("Player teleported to VictoryObject.");
        }
        else
        {
            if (player == null)
            {
                Debug.LogError("Player reference is not assigned! Please assign it in the Inspector.");
            }

            if (victoryObject == null)
            {
                Debug.LogError("VictoryObject reference is not assigned! Please assign it in the Inspector.");
            }
        }
    }
}
