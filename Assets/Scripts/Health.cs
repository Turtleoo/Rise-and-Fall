using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Health : MonoBehaviour
{
    public int maxHealth = 300000000; // Total health points
    private int currentHealth; // Current health points

    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 1f; // Duration of invulnerability
    private float invulnerabilityTimer;

    public GameObject loseMessage; // UI element to display "You Lose!"
    public Button resetButton;     // UI button to reset the game

    private Movement playerMovement; // Reference to the Movement script

    void Start()
    {
        currentHealth = maxHealth;

        if (loseMessage != null)
        {
            loseMessage.SetActive(false); // Hide the lose message initially
        }

        if (resetButton != null)
        {
            resetButton.gameObject.SetActive(false); // Hide the reset button initially
            resetButton.onClick.AddListener(ResetGame); // Add listener to the reset button
        }

        playerMovement = GetComponent<Movement>();
    }

    void Update()
    {
        // Handle invulnerability timer
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0f)
            {
                isInvulnerable = false;
                Debug.Log("Player is no longer invulnerable");
            }
        }

        // Update the position of the reset button to always be over the player
        if (resetButton != null && resetButton.gameObject.activeSelf)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            RectTransform rectTransform = resetButton.GetComponent<RectTransform>();
            rectTransform.position = screenPosition;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable)
        {
            Debug.Log("Player is invulnerable and did not take damage");
            return;
        }

        currentHealth -= amount;
        Debug.Log("Player took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            // Start invulnerability frames
            isInvulnerable = true;
            invulnerabilityTimer = invulnerabilityDuration;
            Debug.Log("Player is now invulnerable for " + invulnerabilityDuration + " seconds");
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        if (loseMessage != null)
        {
            loseMessage.SetActive(true); // Display the lose message
        }

        if (resetButton != null)
        {
            resetButton.gameObject.SetActive(true); // Show the reset button

            // Explicitly set the size of the button's RectTransform
            RectTransform rectTransform = resetButton.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(3000, 1000); // Set desired Width and Height

            // Force a layout rebuild to update the text size
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        // Disable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Stop most game functions but allow UI interaction
        Time.timeScale = 0.0001f;
    }

    public void ResetGame()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
