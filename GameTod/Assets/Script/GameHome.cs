using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // For scene management

public class GameHome : MonoBehaviour
{
    public GameObject upgradePanel;
    public GameObject backpackPanel;
    public GameObject pausePanel; // Reference to the pause panel
    public Button upgradeButton; // Assign the upgrade button in the inspector
    public Button pauseButton; // Assign the pause button in the inspector
    public Button goButton; // Assign the Go button in the inspector

    private RectTransform upgradePanelRectTransform;
    private RectTransform backpackPanelRectTransform;
    private RectTransform pausePanelRectTransform;

    void Start()
    {
        // Ensure the main menu is active and other panels are inactive at the start
        upgradePanel.SetActive(false);
        backpackPanel.SetActive(false);
        pausePanel.SetActive(false);

        // Get RectTransform components
        upgradePanelRectTransform = upgradePanel.GetComponent<RectTransform>();
        backpackPanelRectTransform = backpackPanel.GetComponent<RectTransform>();
        pausePanelRectTransform = pausePanel.GetComponent<RectTransform>();

        // Add listeners to the buttons
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
        pauseButton.onClick.AddListener(TogglePausePanel);
        goButton.onClick.AddListener(GoToNextScene); // Add listener for Go button
    }

    void Update()
    {
        // Toggle the pause panel if Escape is pressed
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
        }

        // Toggle the backpack panel if 'B' is pressed
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
        }

        // Check if the user clicked outside the upgrade panel
        if (upgradePanel.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverPanel(upgradePanelRectTransform))
        {
            CloseUpgradePanel();
        }

        // Check if the user clicked outside the backpack panel
        if (backpackPanel.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverPanel(backpackPanelRectTransform))
        {
            CloseBackpackPanel();
        }

        // Check if the user clicked outside the pause panel
        if (pausePanel.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverPanel(pausePanelRectTransform))
        {
            ClosePausePanel();
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
