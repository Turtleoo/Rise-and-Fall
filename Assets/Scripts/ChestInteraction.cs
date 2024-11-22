using UnityEngine;
using UnityEngine.UI;

public class ChestInteraction : MonoBehaviour
{
    public GameObject prompt; // UI element to display "Press E"
    public GameObject winMessage; // UI element to display "You Win!"
    public Movement playerMovement; // Reference to the Movement script
    public Animator barrelAnimator; // Reference to the barrel's Animator component
    private bool playerInRange = false;

    void Start()
    {
        if (prompt != null)
        {
            prompt.SetActive(false); // Ensure the prompt is hidden initially
        }

        if (winMessage != null)
        {
            winMessage.SetActive(false); // Ensure the win message is hidden initially
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (prompt != null)
            {
                prompt.SetActive(true);
            }
            // Automatically get the Movement component
            if (playerMovement == null)
            {
                playerMovement = other.GetComponent<Movement>();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
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

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (prompt != null)
            {
                prompt.SetActive(false); // Hide the prompt
            }

            if (winMessage != null)
            {
                winMessage.SetActive(true); // Show the win message
            }

            if (playerMovement != null)
            {
                playerMovement.UnlockJump(); // Unlock the ability to jump
                playerMovement.DisableGlide(); // Disable gliding functionality
            }

            // Trigger the barrel's animation
            if (barrelAnimator != null)
            {
                barrelAnimator.SetTrigger("PlayBarrelAnimation");
            }

            Destroy(gameObject);
        }
    }
}
