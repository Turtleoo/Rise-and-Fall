using UnityEngine;

public class Crossbow : MonoBehaviour
{
    public GameObject arrowPrefab;   // Reference to the arrow prefab
    public Transform firePoint;     // Reference to the point where the arrow is fired
    public float arrowSpeed = 10f;  // Speed of the arrow
    public AudioSource fireSound;   // Reference to the AudioSource for the firing sound
    public Collider2D hearingRange; // Collider2D that defines the range where the sound can be heard
    public string playerTag = "Player"; // Tag used to identify the player

    private bool playerInRange = false; // Tracks if the player is within range

    private void Start()
    {
        if (hearingRange == null)
        {
            Debug.LogWarning("Hearing range Collider2D is not assigned!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player entered hearing range.");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player exited hearing range.");
            playerInRange = false;
        }
    }

    public void FireArrow()
    {
        // Instantiate the arrow
        if (arrowPrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

            // Add velocity to the arrow
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.right * arrowSpeed; // Move the arrow in the direction of the firePoint
            }

            // Optional: Add a lifespan to the arrow
            Destroy(arrow, 5f);
        }

        // Play the firing sound only if the player is in range
        if (fireSound != null && playerInRange)
        {
            fireSound.Play();
        }
        else if (fireSound != null && !playerInRange)
        {
            Debug.Log("Player is out of range; no sound played.");
        }
    }
}
