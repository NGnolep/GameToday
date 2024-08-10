using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    public float dashSpeed = 20.0f;  // Speed during dash
    public float dashDuration = 0.2f; // Duration of the dash
    public float dashCooldown = 1.0f; // Cooldown time between dashes
    public float dashBufferTime = 0.2f; // Extra time after dash ends where collisions still count
    public Transform cam; // Reference to the camera's transform

    public Slider energyBar; // UI Slider for energy (also acts as HP)
    public float maxEnergy = 100f; // Maximum energy
    public float currentEnergy; // Current energy
    public float movementEnergyConsumptionRate = 5f; // Energy consumed per second while moving

    public Slider shieldBar; // UI Slider for shield
    public float maxShield = 50f; // Maximum shield
    public float currentShield; // Current shield
    public float shieldRecoveryRate = 5f; // Shield recovery rate per second when not taking damage

    public float strength = 10f; // Player's strength stat
    public float weight = 5f; // Player's weight stat
    public float protection = 5f; // Player's protection stat

    private bool isDashing = false;
    private bool inDashBuffer = false;
    private float dashEndTime;
    private float dashCooldownTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;  // Use Continuous Collision Detection
        currentEnergy = maxEnergy;
        currentShield = maxShield;
        UpdateEnergyBar();
        UpdateShieldBar();
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

        // Recover shield over time when not taking damage
        if (currentShield < maxShield)
        {
            currentShield += shieldRecoveryRate * Time.deltaTime;
            currentShield = Mathf.Clamp(currentShield, 0, maxShield);
            UpdateShieldBar();
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        // Make the movement relative to the camera's direction
        if (movement.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            movement = rotation * Vector3.forward;

            // Reduce energy when moving
            currentEnergy -= movementEnergyConsumptionRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            UpdateEnergyBar();
        }

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

    void UpdateEnergyBar()
    {
        if (energyBar != null)
        {
            energyBar.value = currentEnergy / maxEnergy;
        }
    }

    void UpdateShieldBar()
    {
        if (shieldBar != null)
        {
            shieldBar.value = currentShield / maxShield;
        }
    }

    public void ApplyDamage(float damage)
    {
        float damageAfterProtection = damage - protection;

        if (damageAfterProtection > 0)
        {
            if (currentShield > 0)
            {
                float damageToShield = Mathf.Min(currentShield, damageAfterProtection);
                currentShield -= damageToShield;
                damageAfterProtection -= damageToShield;
                UpdateShieldBar();
            }

            if (damageAfterProtection > 0)
            {
                // Apply remaining damage to energy (HP)
                currentEnergy -= damageAfterProtection;
                currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
                UpdateEnergyBar();

                // If energy reaches 0, the player would be considered "dead" (you can implement logic for that)
                if (currentEnergy <= 0)
                {
                    Debug.Log("Player is out of energy (HP)!");
                    // Implement player death or game over logic here
                }
            }

            Debug.Log("Player took " + damageAfterProtection + " damage after protection and shield.");
        }
    }
}
