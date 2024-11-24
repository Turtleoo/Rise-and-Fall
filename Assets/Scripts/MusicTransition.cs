using UnityEngine;

public class MusicTransition : MonoBehaviour
{
    public AudioSource fallingMusic;  // Reference to the falling music Audio Source
    public AudioSource bottomMusic;   // Reference to the bottom music Audio Source
    public GameObject bottomTrigger;  // Reference to the GameObject with the bottom trigger
    public float fadeDuration = 1.0f; // Duration of the fade (seconds)

    private bool hasReachedBottom = false;

    private void Start()
    {
        // Ensure the trigger GameObject is assigned
        if (bottomTrigger == null)
        {
            Debug.LogError("Bottom trigger GameObject is not assigned!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered by: {other.name}"); // Log to check what is entering the trigger

        // Check if the object entering the trigger has the tag "Player"
        if (other.CompareTag("Player") && !hasReachedBottom)
        {
            Debug.Log("Player reached the bottom. Starting music transition...");
            hasReachedBottom = true;
            StartCoroutine(FadeOutAndSwitch());
        }
    }

    private System.Collections.IEnumerator FadeOutAndSwitch()
    {
        // Fade out the falling music
        float startVolume = fallingMusic.volume;

        while (fallingMusic.volume > 0)
        {
            fallingMusic.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        fallingMusic.Stop();
        fallingMusic.volume = startVolume; // Reset the volume for future use

        // Switch to the bottom music
        bottomMusic.Play();

        // Optional: Fade in the bottom music
        bottomMusic.volume = 0;
        while (bottomMusic.volume < startVolume)
        {
            bottomMusic.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
    }
}
