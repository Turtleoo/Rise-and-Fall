using UnityEngine;
using UnityEngine.Audio;

public class VictoryChest : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject victoryCanvasChild; // UI child of a canvas to display when opened

    [Header("Chest Interaction")]
    public Animator chestAnimator; // Animator for the chest
    public AudioSource chestOpenSound; // Audio effect played when the chest is opened

    [Header("Audio Mixer Settings")]
    public AudioMixer audioMixer1; // First audio mixer to mute
    public AudioMixer audioMixer2; // Second audio mixer to mute
    public string audioMixer1VolumeParam = "MasterVolume"; // Parameter name for first mixer
    public string audioMixer2VolumeParam = "MasterVolume"; // Parameter name for second mixer

    [Header("Interaction Settings")]
    public GameObject prompt; // Prompt UI to display "Press E"
    private bool playerInRange = false;
    private bool chestOpened = false; // Tracks if the chest has already been opened

    private void Start()
    {
        // Ensure the prompt is hidden initially
        if (prompt != null) prompt.SetActive(false);

        // Ensure the victory canvas child is hidden initially
        if (victoryCanvasChild != null) victoryCanvasChild.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !chestOpened) // Only allow interaction if unopened
        {
            playerInRange = true;
            if (prompt != null) prompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (prompt != null) prompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !chestOpened)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        chestOpened = true; // Mark the chest as opened

        // Hide the interaction prompt
        if (prompt != null) prompt.SetActive(false);

        // Play chest open sound effect
        if (chestOpenSound != null)
        {
            chestOpenSound.Play();
        }

        // Trigger chest opening animation
        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("Open");
        }

        // Display the victory canvas child
        if (victoryCanvasChild != null)
        {
            victoryCanvasChild.SetActive(true);
        }

        // Set audio mixers' volume to 0
        if (audioMixer1 != null)
        {
            audioMixer1.SetFloat(audioMixer1VolumeParam, -80f); // Mute volume
        }
        if (audioMixer2 != null)
        {
            audioMixer2.SetFloat(audioMixer2VolumeParam, -80f); // Mute volume
        }

        Debug.Log("Victory chest opened: UI displayed, animation played, and audio muted.");
    }
}
