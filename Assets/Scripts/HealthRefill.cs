using UnityEngine;

public class HealthRefill : MonoBehaviour
{
    public int refillAmount = 1; // Amount of health to refill, can be set in the Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the Health script from the player
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Refill the player's health, but ensure it doesn't exceed maxHealth
                int newHealth = Mathf.Clamp(playerHealth.currentHealth + refillAmount, 0, playerHealth.maxHealth);

                // Check if the health increased
                if (newHealth > playerHealth.currentHealth)
                {
                    playerHealth.currentHealth = newHealth;

                    // Use the existing method in the Health script to update the health bar
                    playerHealth.SendMessage("UpdateHealthBar");

                    Debug.Log("Player's health refilled to: " + newHealth);
                }
                else
                {
                    Debug.Log("Player's health is already full.");
                }
            }

            // Optional: Destroy the health refill object after use
            Destroy(gameObject);
        }
    }
}
