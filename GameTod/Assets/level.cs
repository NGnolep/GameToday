using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public int CurrentLevel { get; private set; } = 1; // Start at level 1
    public bool IsTransitioningToNextLevel { get; set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method when transitioning to the next level
    public void TransitionToNextLevel()
    {
        IsTransitioningToNextLevel = true;
        CurrentLevel++;
        // Notify any other systems if needed
    }

    // Call this method to reset the level
    public void ResetLevel()
    {
        CurrentLevel = 1; // Reset to level 1 or the appropriate starting level
        // Notify any other systems if needed
    }
}
