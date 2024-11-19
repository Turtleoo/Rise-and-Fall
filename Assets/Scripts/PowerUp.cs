using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
            // Get the player's movement script
            Movement playerMovement = collision.GetComponent<Movement>();
            if (playerMovement != null)
            {
                playerMovement.EnableTripleJump(); // Enable the temporary triple jump
            }

            // Destroy the power-up after it has been consumed
            Destroy(gameObject);
        }
    }
}
