using UnityEngine;

public class VerticalOscillator : MonoBehaviour
{
    [Header("Oscillation Settings")]
    [Tooltip("The speed of the vertical oscillation.")]
    public float speed = 2f;

    [Tooltip("The maximum distance the platform moves from its starting position.")]
    public float range = 1f;

    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position of the platform
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * range;

        // Apply the new position to the platform
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
