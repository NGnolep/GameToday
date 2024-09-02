using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public float activationDistance = 3f; // Distance within which the player can interact with the portal

    private void Update()
    {
        // Check if the player is pressing 'E' and is close enough to the portal
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null && Vector3.Distance(player.transform.position, transform.position) <= activationDistance)
            {
                // Trigger the transition to the next scene
                TransitionToNextScene();
            }
        }
    }

    private void TransitionToNextScene()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the index of the next scene
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is valid
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene
            SceneManager.LoadScene(nextSceneIndex);
            Debug.Log($"Transitioning to scene index: {nextSceneIndex}");
        }
        else
        {
            Debug.LogWarning("No next scene available. You are on the last scene in the build settings.");
        }
    }
}
