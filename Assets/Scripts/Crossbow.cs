using UnityEngine;

public class Crossbow : MonoBehaviour
{
    public GameObject arrowPrefab; // Reference to the arrow prefab
    public Transform firePoint;   // Reference to the point where the arrow is fired
    public float arrowSpeed = 10f; // Speed of the arrow

    // This method will be triggered by the animation event
    public void FireArrow()
    {
        if (arrowPrefab != null && firePoint != null)
        {
            // Instantiate the arrow
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
    }
}
