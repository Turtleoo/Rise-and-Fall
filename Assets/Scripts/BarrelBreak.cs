using UnityEngine;

public class BarrelBreak : MonoBehaviour
{
    public GameObject escapeBatPrefab; // The prefab for the EscapeBat
    public int batCount = 10;          // Number of EscapeBats to spawn

    // This function is triggered when the barrel breaks
    public void OnBarrelBreak()
    {
        SpawnEscapeBats();
        Destroy(gameObject); // Destroy the barrel after spawning the bats
    }

    void SpawnEscapeBats()
    {
        if (escapeBatPrefab == null)
        {
            Debug.LogWarning("EscapeBat prefab not assigned!");
            return;
        }

        for (int i = 0; i < batCount; i++)
        {
            // Randomize the position slightly around the barrel's position
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            // Instantiate the EscapeBat prefab
            Instantiate(escapeBatPrefab, spawnPosition, Quaternion.identity);
        }
    }
}