using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;       // Drag the Main Menu panel in the inspector
    public GameObject settingsPanel;       // Drag the Settings panel in the inspector
    public GameObject volumePanel;         // Drag the Volume panel in the inspector
    public GameObject keyBindingsPanel;    // Drag the Key Bindings panel in the inspector
    public Slider volumeSlider;            // Drag the volume slider in the inspector
    public AudioSource audioSource;        // Drag the AudioSource that controls the game’s sound

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the main menu is active and other panels are inactive at the start
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        volumePanel.SetActive(false);
        keyBindingsPanel.SetActive(false);

        // Set the initial slider value to match the current volume
        volumeSlider.value = audioSource.volume;
    }

    // Function for the Play button
    public void PlayGame()
    {
        // Load the next scene in the build order
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    // Function for the Settings button
    public void OpenSettings()
    {
        // Open the settings panel and close the main menu panel
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Function for the Volume button
    public void OpenVolumeSettings()
    {
        // Open the volume panel and close the settings panel
        volumePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // Function for the Key Bindings button
    public void OpenKeyBindings()
    {
        // Open the key bindings panel and close the settings panel
        keyBindingsPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // Function to go back from the Volume or Key Bindings panel to the Settings panel
    public void BackToSettings()
    {
        volumePanel.SetActive(false);
        keyBindingsPanel.SetActive(false);
        settingsPanel.SetActive(true); // Reopen the settings panel when closing the sub-panels
    }

    // Function to go back from the Settings panel to the Main Menu panel
    public void BackToMainMenu()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Function to adjust volume
    public void AdjustVolume()
    {
        // Set the audio source volume to match the slider value
        audioSource.volume = volumeSlider.value;
    }

    // Function for the Quit button
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();

        // This is only useful in the editor to simulate the quit
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
