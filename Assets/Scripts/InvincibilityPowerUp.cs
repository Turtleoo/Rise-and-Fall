using UnityEngine;

public class InvincibilityPowerUp : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobbingAmplitude = 0.5f; // How far the object moves up and down
    public float bobbingSpeed = 2f; // How fast the object moves up and down

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Bobbing effect
        float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Activate invincibility
            InvincibilityManager.Instance.ActivateInvincibility(collision.gameObject);


            // Hide the power-up
            gameObject.SetActive(false);
        }
    }
}
