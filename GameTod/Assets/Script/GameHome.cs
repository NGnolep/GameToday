using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameHome : MonoBehaviour
{
    public GameObject upgradePanel;
    public GameObject backpackPanel;
    public GameObject pausePanel;
    public Button upgradeButton;
    public Button pauseButton;
    public Button goButton;

    public AudioSource backgroundMusicSource;   // AudioSource for background music
    public AudioSource soundEffectSource;       // AudioSource for sound effects
    public AudioClip buttonClickSound;          // Sound for button clicks

    private RectTransform upgradePanelRectTransform;
    private RectTransform backpackPanelRectTransform;
    private RectTransform pausePanelRectTransform;

    void Start()
    {
        // Ensure cursor is visible and not locked
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Ensure UI panels are properly initialized
        upgradePanel.SetActive(false);
        backpackPanel.SetActive(false);
        pausePanel.SetActive(false);

        // Initialize background music
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = 0.5f;  // Adjust volume as needed
            backgroundMusicSource.loop = true;    // Loop the background music

            if (backgroundMusicSource.clip != null)
            {
                backgroundMusicSource.Play();
                Debug.Log("Background music started.");
            }
            else
            {
                Debug.LogWarning("No AudioClip assigned to backgroundMusicSource.");
            }
        }
        else
        {
            Debug.LogError("BackgroundMusicSource is not assigned.");
        }

        // Get RectTransform components
        upgradePanelRectTransform = upgradePanel.GetComponent<RectTransform>();
        backpackPanelRectTransform = backpackPanel.GetComponent<RectTransform>();
        pausePanelRectTransform = pausePanel.GetComponent<RectTransform>();

        // Add listeners to buttons with sound
        upgradeButton.onClick.AddListener(() => { OpenUpgradePanel(); PlayButtonClickSound(); });
        pauseButton.onClick.AddListener(() => { TogglePausePanel(); PlayButtonClickSound(); });
        goButton.onClick.AddListener(() => { GoToNextScene(); PlayButtonClickSound(); });

        // Check EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogError("EventSystem not found in the scene!");
        }
    }

    void Update()
    {
        // Handle Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (upgradePanel.activeSelf)
            {
                CloseUpgradePanel();
            }
            else if (backpackPanel.activeSelf)
            {
                CloseBackpackPanel();
            }
            else if (pausePanel.activeSelf)
            {
                ClosePausePanel();
            }
            else
            {
                OpenPausePanel();
            }

            PlayButtonClickSound();
        }

        // Handle 'B' key press
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (backpackPanel.activeSelf)
            {
                CloseBackpackPanel();
            }
            else
            {
                OpenBackpackPanel();
            }

            PlayButtonClickSound();
        }

        // Check for clicks outside of panels
        if (upgradePanel.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverPanel(upgradePanelRectTransform))
        {
            CloseUpgradePanel();
            PlayButtonClickSound();
        }
        if (backpackPanel.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverPanel(backpackPanelRectTransform))
        {
            CloseBackpackPanel();
            PlayButtonClickSound();
        }
        if (pausePanel.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverPanel(pausePanelRectTransform))
        {
            ClosePausePanel();
            PlayButtonClickSound();
        }
    }

    public void PlayButtonClickSound()
    {
        // Play the button click sound
        if (soundEffectSource != null && buttonClickSound != null)
        {
            soundEffectSource.PlayOneShot(buttonClickSound);
        }
    }

    public void OpenUpgradePanel()
    {
        upgradePanel.SetActive(true);
    }

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }

    public void OpenBackpackPanel()
    {
        backpackPanel.SetActive(true);
    }

    public void CloseBackpackPanel()
    {
        backpackPanel.SetActive(false);
    }

    public void OpenPausePanel()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    public void TogglePausePanel()
    {
        if (pausePanel.activeSelf)
        {
            ClosePausePanel();
        }
        else
        {
            OpenPausePanel();
        }
    }

    public void GoToNextScene()
    {
        // Resume time scale before loading the next scene
        Time.timeScale = 1f;

        // Load the next scene (assuming you have the next scene in build settings)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Helper function to check if the pointer is over a specific UI panel
    private bool IsPointerOverPanel(RectTransform panelRectTransform)
    {
        Vector2 localMousePosition = panelRectTransform.InverseTransformPoint(Input.mousePosition);
        return panelRectTransform.rect.Contains(localMousePosition);
    }

    // Helper function to check if the pointer is over any UI object
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
