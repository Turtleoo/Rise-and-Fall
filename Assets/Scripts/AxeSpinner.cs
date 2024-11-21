using UnityEngine;

public class AxeSpinner : MonoBehaviour
{
    public float rotationSpeed = 150f; // Speed of rotation in degrees per second

    void Update()
    {
        // Rotate the pivot, causing the axe to spin
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
