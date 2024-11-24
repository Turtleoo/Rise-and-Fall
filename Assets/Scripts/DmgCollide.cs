using UnityEngine;

public class DmgCollide : MonoBehaviour
{
    public float bumpForce = 5f; // Adjust this value for the intensity of the bump

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Health playerHealth = collision.collider.GetComponent<Health>();
            Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

            if (playerRb != null)
            {
                // Calculate the bump direction
                Vector2 bumpDirection = (collision.collider.transform.position - transform.position).normalized;

                // Determine the bump force to apply based on the direction
                Vector2 bumpForceVector = new Vector2(
                    bumpDirection.x > 0 ? bumpForce : -bumpForce,  // X direction
                    bumpDirection.y > 0 ? bumpForce : -bumpForce  // Y direction
                );

                // Apply the bump force
                playerRb.AddForce(bumpForceVector, ForceMode2D.Impulse);
            }
        }
    }
}
