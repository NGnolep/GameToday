using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public GameObject player;
    public float mouseSensitivity = 100f; // Sensitivity of the mouse
    public float distanceFromPlayer = 5f; // Distance the camera should maintain from the player

    private float pitch = 0f; // Up/Down rotation
    private float yaw = 0f;   // Left/Right rotation

    void Start()
    {
        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Initialize the camera position
        Vector3 direction = new Vector3(0, 0, -distanceFromPlayer);
        transform.position = player.transform.position + direction;
        transform.LookAt(player.transform.position);
    }

    void LateUpdate()
    {
        // Mouse input for camera rotation
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Clamping pitch to avoid camera flipping
        pitch = Mathf.Clamp(pitch, -45f, 45f);

        // Calculate the new rotation based on mouse input
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Set the camera position and rotation
        Vector3 direction = new Vector3(0, 0, -distanceFromPlayer);
        transform.position = player.transform.position + rotation * direction;
        transform.LookAt(player.transform.position);
    }
}
