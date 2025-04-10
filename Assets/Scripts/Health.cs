using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;

[System.Serializable]
public class AudioMixerParameter
{
    public AudioMixer audioMixer;          // The AudioMixer asset
    public string exposedParameter;        // The name of the exposed parameter in the AudioMixer
    [HideInInspector]
    public float initialValue;             // The initial value of the parameter
}

public class Health : MonoBehaviour
{
    public int maxHealth = 5; // Total health points 
    public int currentHealth; // Current health points

    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 1f; // Duration of invulnerability
    private float invulnerabilityTimer;

    private bool isCompletelyInvincible = false; // Tracks invincibility for TakeDamage bypass

    public GameObject loseMessage; // UI element to display "You Lose!"
    public Button resetButton;     // UI button to reset the game

    private Movement playerMovement; // Reference to the Movement script
    private Animator animator; // Reference to the Animator component

    // Audio sources
    public AudioSource hitAudioSource; // Assign an audio clip for "hit" sound in the Inspector
    public AudioSource deathAudioSource; // Assign an audio clip for "death" sound in the Inspector

    // Audio mixer parameters
    [Header("Audio Mixer Parameter Settings")]
    public AudioMixerParameter[] audioMixerParameters; // List of AudioMixer parameters to manage

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
        animator = GetComponent<Animator>(); // Get the Animator component

        // Initialize Health Bar
        InitializeHealthBar();

        // Store initial AudioMixer parameter values
        if (audioMixerParameters != null)
        {
            foreach (var param in audioMixerParameters)
            {
                if (param.audioMixer != null && !string.IsNullOrEmpty(param.exposedParameter))
                {
                    float value;
                    param.audioMixer.GetFloat(param.exposedParameter, out value);
                    param.initialValue = value;
                    Debug.Log($"Stored initial value for {param.exposedParameter}: {value}");
                }
            }
        }
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
        healthBarParent.transform.localPosition = new Vector3(-6.25f, 4.25f, 0); // Adjust as needed
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
        if (isCompletelyInvincible)
        {
            Debug.Log("Player is completely invincible and did not take damage");
            return; // Bypass damage completely
        }

        if (isInvulnerable)
        {
            Debug.Log("Player is invulnerable and did not take damage");
            return;
        }

        currentHealth -= amount;
        Debug.Log("Player took damage! Current health: " + currentHealth);

        // Play the "Hit" animation
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // Play hit audio
        if (hitAudioSource != null)
        {
            hitAudioSource.Play();
        }

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

    // This allows toggling the invincibility state externally
    public void SetInvincibility(bool state)
    {
        isCompletelyInvincible = state;
        Debug.Log("Player invincibility set to: " + state);
    }


    /// Updates the health bar visuals based on current health.
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

        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
            Debug.Log("Death animation triggered.");
            StartCoroutine(WaitForDeathAnimation());
        }

        // Play death audio
        if (deathAudioSource != null)
        {
            deathAudioSource.Play();
        }

        // Disable player movement immediately
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    /// Coroutine to wait for the death animation to finish before executing the rest of the logic
    private IEnumerator WaitForDeathAnimation()
    {
        if (animator != null)
        {
            // Get the animator state info
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Wait for the animation with the "Die" trigger to finish
            while (stateInfo.IsName("Die") && stateInfo.normalizedTime < 1.0f)
            {
                yield return null; // Wait for the next frame
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            Debug.Log("Death animation completed.");
        }

        // After animation, display the lose message and reset button
        if (loseMessage != null)
        {
            loseMessage.SetActive(true); // Display the lose message
        }

        if (resetButton != null)
        {
            resetButton.gameObject.SetActive(true); // Show the reset button

            // Set the button's position above the player
            Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            RectTransform resetButtonRectTransform = resetButton.GetComponent<RectTransform>();

            // Adjust the offset to place the button above the player
            Vector3 buttonPosition = playerScreenPosition + new Vector3(0, 100, 0); // Offset above the player's screen position
            resetButtonRectTransform.position = buttonPosition;

            // Explicitly set the size of the button's RectTransform if needed
            resetButtonRectTransform.sizeDelta = new Vector2(300, 100); // Set desired width and height
        }

        // Slow down the game only after the animation and UI updates are completed
        yield return new WaitForSeconds(0.5f); // Optional delay before slowing the game down
        Time.timeScale = 0.00001f; // Slow the game
    }

    /// Resets the game by reloading the current scene.
    public void ResetGame()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Restore initial values for all specified audio mixer parameters
        if (audioMixerParameters != null)
        {
            foreach (var param in audioMixerParameters)
            {
                if (param.audioMixer != null && !string.IsNullOrEmpty(param.exposedParameter))
                {
                    param.audioMixer.SetFloat(param.exposedParameter, param.initialValue);
                    Debug.Log($"Restored {param.exposedParameter} to {param.initialValue} dB.");
                }
            }
        }

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
