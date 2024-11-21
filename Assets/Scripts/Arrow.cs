using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb; // Reference to the Rigidbody2D
    private Collider2D arrowCollider; // Reference to the arrow's collider
    private bool isAttached = false; // Whether the arrow is attached to a target

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAttached && collision.gameObject.CompareTag("Player"))
        {
            StickToPlayer(collision.gameObject);
        }
        else if (!isAttached)
        {
            StopArrow(collision.contacts[0].point); // Stop arrow and leave it at the collision point
        }
    }

    void StickToPlayer(GameObject player)
    {
        isAttached = true;

        // Disable Rigidbody and collider for the arrow
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; // Set Rigidbody to Kinematic instead of isKinematic
        arrowCollider.enabled = false;

        // Parent the arrow to the player
        transform.SetParent(player.transform);

        // Position the arrow relative to the player at the point of impact
        transform.localPosition = player.transform.InverseTransformPoint(transform.position);

        // Align the arrow's rotation with its original forward direction
        Vector2 direction = transform.right; // Use the arrow's forward direction (flip if necessary)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Destroy the arrow after 3 seconds
        Destroy(gameObject, 3f);

        Debug.Log($"Arrow Direction: {transform.right}");
    }

    void StopArrow(Vector2 collisionPoint)
    {
        isAttached = true;

        // Disable Rigidbody but keep the collider enabled for visual purposes
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; // Set Rigidbody to Kinematic instead of isKinematic

        // Prevent the arrow from blocking player movement
        arrowCollider.isTrigger = true;

        // Fix the arrow's position at the collision point
        transform.position = collisionPoint;
    }
}