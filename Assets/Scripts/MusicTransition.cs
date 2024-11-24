using UnityEngine;
using System.Collections;

public class MusicTransition : MonoBehaviour
{
    public AudioSource fallingMusic;  // Reference to the falling music Audio Source
    public AudioSource bottomMusic;   // Reference to the bottom music Audio Source
    public GameObject bottomTrigger;  // Reference to the GameObject with the bottom trigger
    public float fadeDuration = 1.0f; // Duration of the fade (seconds)
    public float loopInterval = 2.0f; // Time interval between loops when looping is enabled

    private bool hasReachedBottom = false;
    private Coroutine loopingCoroutine = null; // To track the looping coroutine

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

    private void OnTriggerExit2D(Collider2D other)
    {
        // Stop the bottom music looping when the player leaves the collider
        if (other.CompareTag("Player") && hasReachedBottom)
        {
            Debug.Log("Player exited the trigger area. Stopping bottom music loop.");
            hasReachedBottom = false;

            if (loopingCoroutine != null)
            {
                StopCoroutine(loopingCoroutine);
                loopingCoroutine = null;
            }

            if (bottomMusic.isPlaying)
            {
                bottomMusic.Stop();
            }
        }
    }

    private IEnumerator FadeOutAndSwitch()
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
        if (bottomMusic.loop)
        {
            // Stop Unity's default looping and handle it manually
            bottomMusic.loop = false;
            loopingCoroutine = StartCoroutine(PlayMusicWithInterval(bottomMusic));
        }
        else
        {
            bottomMusic.Play();
        }

        // Optional: Fade in the bottom music
        bottomMusic.volume = 0;
        while (bottomMusic.volume < startVolume)
        {
            bottomMusic.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    private IEnumerator PlayMusicWithInterval(AudioSource music)
    {
        while (true)
        {
            music.Play();

            // Wait for the clip duration + the loop interval
            yield return new WaitForSeconds(music.clip.length + loopInterval);

            music.Stop();
        }
    }
}
