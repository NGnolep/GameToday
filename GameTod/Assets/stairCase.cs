using UnityEngine;
using UnityEngine.SceneManagement;

public class Staircase : MonoBehaviour
{
    public float interactionRange = 3f; // Range within which the player can interact with the staircase
    private Transform player; // Reference to the player’s transform

    private bool isPlayerNear = false; // Flag to check if player is near the staircase

    void Start()
    {
        // Find the player object dynamically
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Player not found. Ensure the player GameObject has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player == null) return; // Exit if player is not assigned

        // Check if the player is within interaction range
        if (Vector3.Distance(transform.position, player.position) <= interactionRange)
        {
            isPlayerNear = true;
        }
        else
        {
            isPlayerNear = false;
        }

        // If the player is near the staircase and presses 'E', transition to the Game Home scene
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("Game Home"); // Replace with your exact scene name
        }
    }
}
