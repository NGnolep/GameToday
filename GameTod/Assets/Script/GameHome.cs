using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For Button references
using UnityEngine.EventSystems; // For detecting clicks outside the panel

public class GameHome : MonoBehaviour
{
    public GameObject upgradePanel;
    public GameObject backpackPanel;
    public Button upgradeButton; // Assign the upgrade button in the inspector

    void Start()
    {
        // Ensure the main menu is active and other panels are inactive at the start
        upgradePanel.SetActive(false);
        backpackPanel.SetActive(false);

        // Add listener to the upgrade button
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
    }

    void Update()
    {
        // Close the upgrade panel if Escape is pressed
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
        if (upgradePanel.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            CloseUpgradePanel();
        }

        // Check if the user clicked outside the backpack panel
        if (backpackPanel.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            CloseBackpackPanel();
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

    // Helper function to check if the pointer is over a UI object
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
