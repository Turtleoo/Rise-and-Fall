using UnityEngine;

public class OscillatePlatform : MonoBehaviour
{
    [SerializeField]
    private float oscillationDistance = 2f; // The distance to oscillate

    [SerializeField]
    private float oscillationSpeed = 2f; // The speed of oscillation

    private Vector3 startingPosition;
    private Vector3 platformVelocity; // To track platform velocity
    private Vector3 previousPosition; // Store the previous position

    void Start()
    {
        // Store the starting position of the platform
        startingPosition = transform.position;
        previousPosition = startingPosition;
    }

    void Update()
    {
        // Calculate the new position using a sine wave
        float offset = Mathf.Sin(Time.time * oscillationSpeed) * oscillationDistance;
        Vector3 newPosition = startingPosition + new Vector3(offset, 0f, 0f);

        // Calculate velocity
        platformVelocity = (newPosition - previousPosition) / Time.deltaTime;

        // Update the position
        transform.position = newPosition;

        // Update previous position for the next frame
        previousPosition = newPosition;
    }

    public Vector3 GetPlatformVelocity()
    {
        return platformVelocity;
    }
}
