using UnityEngine;

public class CamController : MonoBehaviour
{
    public GameObject player;
    public float mouseSensitivity = 100f;
    public float distanceFromPlayer = 5f;
    public float minDistanceFromPlayer = 1f;
    public LayerMask collisionMask;

    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (player != null)
        {
            InitializeCameraPosition();
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            HandleCameraRotation();
            HandleCameraCollision();
        }
    }

    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
        InitializeCameraPosition();
    }

    void InitializeCameraPosition()
    {
        Vector3 direction = new Vector3(0, 0, -distanceFromPlayer);
        transform.position = player.transform.position + direction;
        transform.LookAt(player.transform.position);
    }

    void HandleCameraRotation()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -45f, 45f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredCameraPosition = player.transform.position + rotation * new Vector3(0, 0, -distanceFromPlayer);
        transform.position = desiredCameraPosition;
        transform.LookAt(player.transform.position);
    }

    void HandleCameraCollision()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, transform.position - player.transform.position, out hit, distanceFromPlayer, collisionMask))
        {
            float hitDistance = hit.distance;
            Vector3 collisionPosition = player.transform.position + (transform.position - player.transform.position).normalized * hitDistance;
            transform.position = collisionPosition;
        }
    }
}
