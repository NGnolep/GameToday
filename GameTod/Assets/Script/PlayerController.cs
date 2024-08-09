using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    public float dashSpeed = 20.0f;  // Speed during dash
    public float dashDuration = 0.2f; // Duration of the dash
    public float dashCooldown = 1.0f; // Cooldown time between dashes
    public float dashBufferTime = 0.2f; // Extra time after dash ends where collisions still count

    private bool isDashing = false;
    private bool inDashBuffer = false;
    private float dashEndTime;
    private float dashCooldownTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;  // Use Continuous Collision Detection
    }

    void Update()
    {
        // Handle dash input
        if (Input.GetMouseButtonDown(1) && Time.time >= dashCooldownTime)
        {
            isDashing = true;
            dashEndTime = Time.time + dashDuration;
            dashCooldownTime = Time.time + dashCooldown;
            inDashBuffer = false;
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (isDashing)
        {
            rb.AddForce(movement * dashSpeed, ForceMode.Impulse);  // Apply dash speed

            // Check if dash time is over and enter buffer period
            if (Time.time >= dashEndTime)
            {
                isDashing = false;
                inDashBuffer = true;
                dashEndTime = Time.time + dashBufferTime; // Set the buffer end time
                rb.velocity = Vector3.zero;  // Set velocity to zero after dash ends
            }
        }
        else if (inDashBuffer)
        {
            // If in buffer period, check if buffer time has elapsed
            if (Time.time >= dashEndTime)
            {
                inDashBuffer = false;  // End the buffer period
            }
        }
        else
        {
            rb.AddForce(movement * speed);  // Normal movement
        }
    }

    public bool IsDashing()
    {
        return isDashing || inDashBuffer;  // Consider the player "dashing" during the buffer period
    }
}
