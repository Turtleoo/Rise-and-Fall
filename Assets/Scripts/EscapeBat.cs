using UnityEngine;

public class EscapeBat : MonoBehaviour
{
    [SerializeField] private Animator batAnimator; // Reference to the bat's Animator
    [SerializeField] private float batSpeed = 2f;  // Speed at which the bat flies towards the player
    [SerializeField] private Collider2D attackCollider; // Reference to the Polygon Collider 2D (Attack hitbox)
    [SerializeField] private Collider2D aggroCollider;  // Reference to the Capsule Collider 2D (Aggro range)
    [SerializeField] private float rngChangeInterval = 0.5f; // Interval for random movement changes
    [SerializeField] private float rngMovementRange = 3f;    // Range of erratic movement

    private Transform playerTransform;  // Reference to the player's transform
    private bool isFlying = false;      // To track if the bat is in flight
    private Vector2 randomOffset;       // Randomized erratic movement offset
    private float rngTimer;             // Timer for changing RNG movement

    private void Start()
    {
        if (attackCollider == null || aggroCollider == null)
        {
            Debug.LogError("Please assign both Attack Collider and Aggro Collider in the Inspector.");
        }

        // Initialize RNG movement offset
        randomOffset = Vector2.zero;
        rngTimer = rngChangeInterval;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Trigger aggro when the player enters the aggro collider
        if (aggroCollider != null && aggroCollider.bounds.Contains(other.transform.position) && other.CompareTag("Player"))
        {
            // Set the player's transform for flight direction
            playerTransform = other.transform;


            isFlying = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Apply damage when the player is inside the attack collider
        if (attackCollider != null && attackCollider.bounds.Contains(other.transform.position) && other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Apply damage
                playerHealth.TakeDamage(1);
            }
        }
    }

    private void Update()
    {
        if (isFlying && playerTransform != null)
        {
            // Update RNG movement timer
            rngTimer -= Time.deltaTime;
            if (rngTimer <= 0)
            {
                // Generate a new random offset
                randomOffset = new Vector2(
                    Random.Range(-rngMovementRange, rngMovementRange),
                    Random.Range(-rngMovementRange, rngMovementRange)
                );
                rngTimer = rngChangeInterval; // Reset the timer
            }

            // Calculate the spooky, erratic target position
            Vector2 targetPosition = (Vector2)playerTransform.position + randomOffset;

            // Move the bat towards the randomized target position
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                batSpeed * Time.deltaTime
            );
        }
    }
}
