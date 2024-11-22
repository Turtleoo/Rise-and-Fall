using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int maxHealth = 5; // Total health points 
    private int currentHealth; // Current health points

    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 1f; // Duration of invulnerability
    private float invulnerabilityTimer;

    public GameObject loseMessage; // UI element to display "You Lose!"
    public Button resetButton;     // UI button to reset the game

    private Movement playerMovement; // Reference to the Movement script

    // Health bar related
    [Header("Health Bar Settings")]
    public GameObject heartFillPrefab;   // Assign the heart_fill prefab in the Inspector
    public GameObject heartEmptyPrefab;  // Assign the heart_empty prefab in the Inspector
    public float heartSpacing = 0.75f;    // Space between hearts
    private GameObject[] heartObjects;    // Array to hold heart instances
    private Transform cameraTransform;    // Reference to the camera's transform

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

        // Initialize Health Bar
        InitializeHealthBar();
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

        // Optional: Additional update logic
    }

    /// Initializes the health bar by instantiating heart prefabs.
    void InitializeHealthBar()
    {
        // Find the main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found. Please ensure your camera is tagged as 'MainCamera'.");
            return;
        }

        cameraTransform = mainCamera.transform;

        // Create a parent GameObject for the health bar
        GameObject healthBarParent = new GameObject("HealthBar");
        healthBarParent.transform.SetParent(cameraTransform);
        healthBarParent.transform.localPosition = new Vector3(4.25f, 4.25f, 0); // Adjust as needed
        healthBarParent.transform.localRotation = Quaternion.identity;
        healthBarParent.transform.localScale = Vector3.one;

        // Initialize the hearts array
        heartObjects = new GameObject[maxHealth];

        // Instantiate heart_fill prefabs
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartFillPrefab, healthBarParent.transform);

            // Position hearts horizontally with spacing
            heart.transform.localPosition = new Vector3(i * heartSpacing, 0, 10);

            // Scale the hearts
            heart.transform.localScale = new Vector3(0.75f, 0.75f, 1.0f); // Adjust size as needed

            heartObjects[i] = heart;
        }
    }

    /// Reduces the player's health by the specified amount.
    public void TakeDamage(int amount)
    {
        if (isInvulnerable)
        {
            Debug.Log("Player is invulnerable and did not take damage");
            return;
        }

        currentHealth -= amount;
        Debug.Log("Player took damage! Current health: " + currentHealth);

        UpdateHealthBar();

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

    /// Updates the health bar visuals based on current health.
    /// Replaces heart_fill with heart_empty from right to left.
    void UpdateHealthBar()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if (i < currentHealth)
            {
                // Ensure the heart is filled
                if (heartObjects[i].name.Contains("heart_empty"))
                {
                    ReplaceHeart(i, heartFillPrefab);
                }
            }
            else
            {
                // Ensure the heart is empty
                if (heartObjects[i].name.Contains("heart_fill"))
                {
                    ReplaceHeart(i, heartEmptyPrefab);
                }
            }
        }
    }

    /// Replaces a heart GameObject with the specified prefab.
    void ReplaceHeart(int index, GameObject newPrefab)
    {
        if (heartObjects[index] == null)
            return;

        // Store the current position and parent
        Vector3 position = heartObjects[index].transform.localPosition;
        Transform parent = heartObjects[index].transform.parent;

        // Destroy the old heart
        Destroy(heartObjects[index]);

        // Instantiate the new heart
        GameObject newHeart = Instantiate(newPrefab, parent);
        newHeart.transform.localPosition = position;
        newHeart.transform.localRotation = Quaternion.identity;

        // Set the customizable scale for the new heart
        newHeart.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f); // Adjust size as needed

        // Update the reference in the array
        heartObjects[index] = newHeart;
    }

    /// Handles the player's death.
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

    /// Resets the game by reloading the current scene.
    public void ResetGame()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
