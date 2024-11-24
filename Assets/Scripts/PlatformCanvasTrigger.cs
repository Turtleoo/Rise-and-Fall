using UnityEngine;

public class PlatformCanvasTrigger : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject uiElement; // The specific child UI element to display (e.g., EPromptPanel)
    public Transform characterTransform; // The player's Transform
    public float uiOffset = 1.5f; // Vertical offset above the character

    private bool isPlayerOnPlatform = false;

    void Start()
    {
        // Ensure the UI element is initially hidden
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
        else
        {
            Debug.LogError("UI Element is not assigned!");
        }
    }

    void Update()
    {
        // If the player is on the platform, update the UI element's position
        if (isPlayerOnPlatform && uiElement != null)
        {
            Vector3 newUIPosition = new Vector3(
                characterTransform.position.x,
                characterTransform.position.y + uiOffset,
                characterTransform.position.z
            );

            uiElement.transform.position = newUIPosition;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;

            if (uiElement != null)
            {
                uiElement.SetActive(true);
            }

            Debug.Log("Player stepped on the platform. UI element displayed.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;

            if (uiElement != null)
            {
                uiElement.SetActive(false);
            }

            Debug.Log("Player left the platform. UI element hidden.");
        }
    }
}
