using UnityEngine;

public class BatBehavior: MonoBehaviour
{
    [SerializeField] private Animator batAnimator; // Reference to the bat's Animator
    [SerializeField] private float batSpeed = 2f;  // Speed at which the bat flies towards the player

    private Transform playerTransform;  // Reference to the player's transform
    private bool isFlying = false;     // To track if the bat is in flight

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the trigger.");

            // Set the player's transform for flight direction
            playerTransform = other.transform;

            // Trigger the flying animation
            batAnimator.SetBool("isFlying", true);
            isFlying = true;
        }
    }

    private void Update()
    {
        // If the bat is flying, move it towards the player
        if (isFlying && playerTransform != null)
        {
            // Move towards the player with the specified speed
            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTransform.position,
                batSpeed * Time.deltaTime
            );
        }
    }
}
