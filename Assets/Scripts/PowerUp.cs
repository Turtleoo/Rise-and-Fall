using UnityEngine;
using System.Collections.Generic;

public class PowerUp : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobbingAmplitude = 0.5f; // How far the object moves up and down
    public float bobbingSpeed = 2f; // How fast the object moves up and down

    [Header("Animated Effect")]
    public GameObject animatedEffectObject; // Object with an animation attached (starts hidden)

    [Header("Parent Objects")]
    public List<GameObject> parentObjects; // List of parent objects whose child colliders will be disabled

    private Vector3 startPosition; // The starting position of the power-up

    private void Start()
    {
        // Store the starting position for bobbing
        startPosition = transform.position;

        // Enable the animation object if it's assigned
        if (animatedEffectObject != null)
        {
            animatedEffectObject.SetActive(true);
        }
    }

    private void Update()
    {
        // Bobbing effect: Move the object up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Trigger the ghost ability in the GhostManager
            GhostManager.Instance.ActivateGhostAbility(collision.gameObject, 3f, parentObjects, 0.5f);

            // Hide the power-up
            gameObject.SetActive(false);
        }
    }
}
