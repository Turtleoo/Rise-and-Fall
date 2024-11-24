using UnityEngine;

public class FallingPlatformTrap : MonoBehaviour
{
    public GameObject platform; // Reference to the platform object
    public float fallSpeed = 2.0f; // Speed at which the platform falls
    private bool shouldFall = false; // Flag to determine if the platform should move

    private void Start()
    {
        // Ensure the platform is initially invisible
        if (platform != null)
        {
            platform.SetActive(false);
        }
        else
        {
            Debug.LogError("Platform GameObject is not assigned!");
        }
    }

    private void Update()
    {
        // If the platform should fall, move it down along the Y-axis
        if (shouldFall && platform != null)
        {
            platform.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the trigger is tagged as "Player"
        if (collision.CompareTag("Player") && platform != null)
        {
            // Make the platform visible and trigger the fall
            platform.SetActive(true);
            shouldFall = true;
        }
    }
}
