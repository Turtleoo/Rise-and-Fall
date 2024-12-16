using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private GameObject cameraSubject; // The player or main object the camera follows
    [SerializeField] private float followOffsetZ = -5f; // Offset for the camera's Z position

    [Header("Camera Pan Settings")]
    [SerializeField] public float panSpeed = 2f;      // Speed of the camera pan
    [SerializeField] public float focusDuration = 2f; // How long the camera stays focused on the target

    private Vector3 originalPosition;                 // Original camera position
    private bool isPanning = false;                   // Whether the camera is currently panning

    void LateUpdate()
    {
        if (!isPanning)
        {
            // Regular follow logic: camera follows the player
            if (cameraSubject != null)
            {
                transform.position = cameraSubject.transform.position + new Vector3(0, 0, followOffsetZ);
            }
        }
    }

    /// <summary>
    /// Starts the camera pan to a specific target and returns to the original position.
    /// </summary>
    /// <param name="targetPosition">The world position to pan to.</param>
    /// <param name="panDuration">The duration of the pan to the target.</param>
    public void StartCameraPanToTarget(Vector3 targetPosition, float panDuration)
    {
        if (!isPanning)
        {
            StartCoroutine(PanToSpecificTarget(targetPosition, panDuration));
        }
    }

    private IEnumerator PanToSpecificTarget(Vector3 targetPosition, float panDuration)
    {
        isPanning = true;

        // Save the original position to return to later
        originalPosition = transform.position;

        // Smoothly move to the focus target
        yield return StartCoroutine(SmoothPan(transform.position, targetPosition));

        // Hold the camera at the focus target for the specified duration
        yield return new WaitForSeconds(focusDuration);

        // Smoothly move back to the original position
        yield return StartCoroutine(SmoothPan(transform.position, originalPosition));

        isPanning = false;
    }

    /// <summary>
    /// Smoothly pans the camera from the start position to the target position.
    /// </summary>
    /// <param name="start">The starting position of the camera.</param>
    /// <param name="target">The target position of the camera.</param>
    private IEnumerator SmoothPan(Vector3 start, Vector3 target)
    {
        float elapsedTime = 0f;
        Vector3 adjustedTarget = new Vector3(target.x, target.y, start.z); // Preserve Z position

        while (elapsedTime < panSpeed)
        {
            transform.position = Vector3.Lerp(start, adjustedTarget, elapsedTime / panSpeed);
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        transform.position = adjustedTarget; // Ensure exact position
    }

}
