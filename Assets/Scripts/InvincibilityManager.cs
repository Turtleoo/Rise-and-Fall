using UnityEngine;
using System.Collections;

public class InvincibilityManager : MonoBehaviour
{
    public static InvincibilityManager Instance;

    [Header("Audio Sources")]
    public AudioSource startInvincibilityAudioSource; // AudioSource for invincibility start sound
    public AudioSource endInvincibilityAudioSource;   // AudioSource for invincibility end sound

    [Header("Player Color Settings")]
    public Color invincibilityColor = new Color(250 / 255f, 0f, 255 / 255f, 1f); // FA00FF color
    private Color originalColor; // To store the original player color

    [Header("Animation Object")]
    public GameObject animationObject; // Object to appear when invincibility is active (starts hidden)

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 5f; // Settable duration of invincibility

    private SpriteRenderer[] playerRenderers; // References to all sprite renderers on the player
    private bool isInvincible = false;       // Tracks invincibility state

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateInvincibility(GameObject player)
    {
        if (!isInvincible) // Prevent multiple activations
        {
            StartCoroutine(InvincibilityCoroutine(player));
        }
    }

    private IEnumerator InvincibilityCoroutine(GameObject player)
    {
        isInvincible = true;

        // Play start invincibility sound
        PlayAudio(startInvincibilityAudioSource);

        // Show the animated object and wait for the animation to finish
        yield return StartCoroutine(PlayAnimation());

        // Get player renderers and save original color
        playerRenderers = player.GetComponentsInChildren<SpriteRenderer>();
        if (playerRenderers.Length > 0)
        {
            originalColor = playerRenderers[0].color; // Assume all sprites share the same original color
        }

        // Change the player's color to invincibility color
        SetPlayerColor(invincibilityColor);

        // Disable damage in the Health script
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.SetInvincibility(true);
        }

        // Wait for the invincibility duration
        yield return new WaitForSecondsRealtime(invincibilityDuration);

        // Restore original player color
        SetPlayerColor(originalColor);

        // Re-enable damage in the Health script
        if (playerHealth != null)
        {
            playerHealth.SetInvincibility(false);
        }

        // Play end invincibility sound
        PlayAudio(endInvincibilityAudioSource);

        isInvincible = false;
        Debug.Log("Invincibility ended.");
    }

    private IEnumerator PlayAnimation()
    {
        if (animationObject != null)
        {
            // Activate the animation object
            animationObject.SetActive(true);

            // Wait for the animation to finish playing
            Animator anim = animationObject.GetComponent<Animator>();
            if (anim != null)
            {
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            }
            else
            {
                // Fallback: Wait 1 second if no animator is attached
                yield return new WaitForSeconds(1f);
            }

            // Deactivate the animation object
            animationObject.SetActive(false);
        }
    }

    private void SetPlayerColor(Color color)
    {
        foreach (SpriteRenderer renderer in playerRenderers)
        {
            if (renderer != null)
            {
                renderer.color = color;
            }
        }
    }

    private void PlayAudio(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
