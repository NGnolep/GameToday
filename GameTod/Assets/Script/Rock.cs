using UnityEngine;

public class Rock : MonoBehaviour
{
    public int hp = 50;  // Starting HP for the rock
    public GameObject Stone;
    public GameObject Stone1;
    public GameObject portalPrefab;  // Assign the portal prefab in the inspector
    public Transform portalSpawnPoint;  // Assign a position for the portal to spawn, or use the rock's position
    [Range(0, 100)] public float portalSpawnChance = 15f; // Set the percentage chance for a portal to spawn
    public GameObject orePrefab; // Reference to the ore prefab associated with this rock

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player.IsDashing())  // Only reduce health if the player is dashing
            {
                hp -= 10;

                if (hp <= 0)
                {
                    Debug.Log("Rock destroyed!");
                    Destroy(Stone);
                    Destroy(Stone1); // Destroy the rock

                    // Add ore to inventory
                    InventoryManager.Instance.AddOre(orePrefab); // Add the destroyed ore to the inventory

                    TrySpawnPortal(); // Attempt to spawn a portal when the rock is destroyed
                }
                else
                {
                    Debug.Log("Rock HP: " + hp);
                }
            }
        }
    }

    void TrySpawnPortal()
    {
        // Random chance to spawn a portal
        float randomValue = Random.Range(0f, 100f);
        if (randomValue <= portalSpawnChance)
        {
            Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);
            Debug.Log("Portal to the next floor has appeared!");
        }
    }
}
