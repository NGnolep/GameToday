using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public GameObject player;
    public float mouseSensitivity = 100f; // Sensitivity of the mouse
    public float distanceFromPlayer = 5f; // Default distance the camera should maintain from the player
    public float minDistanceFromPlayer = 1f; // Minimum distance when colliding
    public LayerMask collisionMask; // Layer mask for objects the camera should collide with

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

        // Calculate the desired camera position based on rotation and distance
        Vector3 desiredCameraPosition = player.transform.position + rotation * new Vector3(0, 0, -distanceFromPlayer);

        // Check if the camera collides with any objects
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, desiredCameraPosition - player.transform.position, out hit, distanceFromPlayer, collisionMask))
        {
            // If a collision is detected, move the camera closer to the player
            float hitDistance = hit.distance;
            Vector3 collisionPosition = player.transform.position + rotation * new Vector3(0, 0, -hitDistance);

            // Ensure the camera doesn't zoom in too close
            transform.position = Vector3.Lerp(collisionPosition, player.transform.position, minDistanceFromPlayer / hitDistance);
        }
        else
        {
            // If no collision, maintain the default distance from the player
            transform.position = desiredCameraPosition;
        }

        // Make the camera look at the player
        transform.LookAt(player.transform.position);
    }
}
