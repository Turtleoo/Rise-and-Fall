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

    // Movement bounds
    private readonly Vector2 minBounds = new Vector2(-6f, float.MinValue); // No lower limit for Y
    private readonly Vector2 maxBounds = new Vector2(8f, -1.4f);          // Upper limit for Y is -1.4

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
        if (aggroCollider != null && aggroCollider.bounds.Contains(other.transform.position) && other.CompareTag("Player"))
        {
            playerTransform = other.transform; // Set the player's transform for flight direction
            isFlying = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (attackCollider != null && attackCollider.bounds.Contains(other.transform.position) && other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
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
                randomOffset = new Vector2(
                    Random.Range(-rngMovementRange, rngMovementRange),
                    Random.Range(-rngMovementRange, rngMovementRange)
                );
                rngTimer = rngChangeInterval; // Reset the timer
            }

            Vector2 targetPosition = (Vector2)playerTransform.position + randomOffset;

            // Move towards the randomized target position
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                batSpeed * Time.deltaTime
            );

            // Apply clamping when the lever is inactive
            if (!Lever.IsLeverActive)
            {
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                    Mathf.Clamp(transform.position.y, float.MinValue, maxBounds.y),
                    transform.position.z
                );
            }
        }
    }
}
