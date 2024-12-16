using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance;

    [Header("Audio Sources")]
    public AudioSource startGhostAudioSource; // AudioSource to play when ghost form starts
    public AudioSource endGhostAudioSource; // AudioSource to play when ghost form ends

    [Header("Animation Object")]
    public GameObject animationObject; // The animation object to control (starts hidden)

    private List<Collider2D> disabledColliders = new List<Collider2D>(); // Track colliders disabled during ghost mode

    private void Awake()
    {
        // Singleton pattern
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

    public void ActivateGhostAbility(GameObject player, float duration, List<GameObject> parentObjects, float transparency)
    {
        StartCoroutine(GhostAbilityCoroutine(player, duration, parentObjects, transparency));
    }

    private IEnumerator GhostAbilityCoroutine(GameObject player, float duration, List<GameObject> parentObjects, float transparency)
    {
        // Play the start ghost sound
        PlayAudio(startGhostAudioSource);

        // Show the animation
        yield return StartCoroutine(PlayAnimation());

        // Set player transparency
        SpriteRenderer[] playerSpriteRenderers = player.GetComponentsInChildren<SpriteRenderer>();
        SetPlayerTransparency(playerSpriteRenderers, transparency);

        // Disable colliders for all children of the parent objects
        foreach (var parent in parentObjects)
        {
            DisableChildColliders(parent);
        }

        // Wait for the ghost duration
        yield return new WaitForSecondsRealtime(duration);

        // Play the end ghost sound
        PlayAudio(endGhostAudioSource);

        // Restore player transparency
        SetPlayerTransparency(playerSpriteRenderers, 1f);

        // Re-enable child colliders
        ReEnableChildColliders();

        Debug.Log("Ghost ability ended.");
    }

    private void SetPlayerTransparency(SpriteRenderer[] renderers, float alpha)
    {
        foreach (SpriteRenderer sr in renderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = alpha;
                sr.color = color;
            }
        }
    }

    private void DisableChildColliders(GameObject parent)
    {
        if (parent == null) return;

        Collider2D[] colliders = parent.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            if (collider != null && collider.enabled)
            {
                collider.enabled = false; // Disable the collider
                disabledColliders.Add(collider); // Add to the list for re-enabling later
            }
        }
    }

    private void ReEnableChildColliders()
    {
        foreach (Collider2D collider in disabledColliders)
        {
            if (collider != null)
            {
                collider.enabled = true; // Re-enable the collider
            }
        }
        disabledColliders.Clear(); // Clear the list after re-enabling
    }

    private void PlayAudio(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
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
}
