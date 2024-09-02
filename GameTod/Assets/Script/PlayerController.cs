using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    public float dashSpeed = 20.0f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;
    public float dashBufferTime = 0.2f;
    public Transform cam;

    public Slider energyBar; // UI Slider for energy (fuel)
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float movementEnergyConsumptionRate = 5f;

    public Slider shieldBar; // UI Slider for shield
    public float maxShield = 50f;
    public float currentShield;
    public float shieldRecoveryRate = 5f;

    public float strength = 10f;
    public float weight = 5f;
    public float protection = 5f;

    public float invincibilityDuration = 2.0f; // Duration of invincibility
    private bool isInvincible = false;
    private float invincibilityEndTime;

    private bool isDashing = false;
    private bool inDashBuffer = false;
    private float dashEndTime;
    private float dashCooldownTime;

    private AudioSource movementSoundSource;  // Sound for movement
    private AudioSource shieldBreakSoundSource;  // Sound for shield break
    private AudioSource damageSoundSource;  // Sound for player damage
    private AudioSource bgmSource;  // Normal BGM
    private AudioSource lowHealthBgmSource;  // Low health BGM

    public float lowHealthThreshold = 20f;  // Threshold to trigger low health BGM

    void Awake()
    {
        if (energyBar == null)
        {
            energyBar = GameObject.Find("EnergyBar").GetComponent<Slider>();
        }
        if (shieldBar == null)
        {
            shieldBar = GameObject.Find("ShieldBar").GetComponent<Slider>();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        currentEnergy = maxEnergy;
        currentShield = maxShield;
        UpdateEnergyBar();
        UpdateShieldBar();

        if (cam == null)
        {
            cam = Camera.main.transform;
        }

        if (energyBar != null)
        {
            energyBar.minValue = 0;
            energyBar.maxValue = maxEnergy;
            energyBar.value = currentEnergy;
        }

        if (shieldBar != null)
        {
            shieldBar.minValue = 0;
            shieldBar.maxValue = maxShield;
            shieldBar.value = currentShield;
        }

        movementSoundSource = GameObject.Find("MovementSound").GetComponent<AudioSource>();
        shieldBreakSoundSource = GameObject.Find("ShieldBreakSound").GetComponent<AudioSource>();
        damageSoundSource = GameObject.Find("DamageSound").GetComponent<AudioSource>();
        bgmSource = GameObject.Find("BGM").GetComponent<AudioSource>();
        lowHealthBgmSource = GameObject.Find("LowHealthBGM").GetComponent<AudioSource>();

        if (movementSoundSource != null)
        {
            movementSoundSource.loop = true;
            movementSoundSource.Stop();
        }

        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    void Update()
    {
        if (isInvincible && Time.time >= invincibilityEndTime)
        {
            isInvincible = false;
        }

        if (Input.GetMouseButtonDown(1) && Time.time >= dashCooldownTime)
        {
            isDashing = true;
            dashEndTime = Time.time + dashDuration;
            dashCooldownTime = Time.time + dashCooldown;
            inDashBuffer = false;
        }

        if (currentShield < maxShield)
        {
            currentShield += shieldRecoveryRate * Time.deltaTime;
            currentShield = Mathf.Clamp(currentShield, 0, maxShield);
            UpdateShieldBar();
        }

        if (movementSoundSource != null)
        {
            if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !movementSoundSource.isPlaying)
            {
                Debug.Log("Playing movement sound");
                movementSoundSource.Play();
            }
            else if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && movementSoundSource.isPlaying)
            {
                Debug.Log("Stopping movement sound");
                movementSoundSource.Stop();
            }
        }

        if (currentEnergy <= lowHealthThreshold && lowHealthBgmSource != null && !lowHealthBgmSource.isPlaying)
        {
            if (bgmSource != null && bgmSource.isPlaying)
            {
                Debug.Log("Stopping normal BGM");
                bgmSource.Stop();  // Stop the normal BGM
            }
            Debug.Log("Playing low health BGM");
            lowHealthBgmSource.loop = true;
            lowHealthBgmSource.Play();  // Start low health BGM
        }
        else if (currentEnergy > lowHealthThreshold && lowHealthBgmSource != null && lowHealthBgmSource.isPlaying)
        {
            Debug.Log("Stopping low health BGM");
            lowHealthBgmSource.Stop();  // Stop low health BGM
            if (bgmSource != null)
            {
                Debug.Log("Resuming normal BGM");
                bgmSource.Play();  // Resume normal BGM
            }
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        if (movement.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            movement = rotation * Vector3.forward;
            currentEnergy -= movementEnergyConsumptionRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            UpdateEnergyBar();
        }

        if (isDashing)
        {
            rb.AddForce(movement * dashSpeed, ForceMode.Impulse);

            if (Time.time >= dashEndTime)
            {
                isDashing = false;
                inDashBuffer = true;
                dashEndTime = Time.time + dashBufferTime;
                rb.velocity = Vector3.zero;
            }
        }
        else if (inDashBuffer)
        {
            if (Time.time >= dashEndTime)
            {
                inDashBuffer = false;
            }
        }
        else
        {
            rb.AddForce(movement * speed);
        }
    }

    public bool IsDashing()
    {
        return isDashing || inDashBuffer;
    }

    void UpdateEnergyBar()
    {
        if (energyBar != null)
        {
            energyBar.value = currentEnergy;
        }
    }

    void UpdateShieldBar()
    {
        if (shieldBar != null)
        {
            Debug.Log("Updating shield bar to: " + currentShield);
            shieldBar.value = currentShield;
        }
    }

    public void ApplyDamage(float damage)
    {
        if (isInvincible)
        {
            return; // Skip damage application if the player is invincible
        }

        float damageAfterProtection = damage - protection;

        if (damageAfterProtection > 0)
        {
            if (currentShield > 0)
            {
                float damageToShield = Mathf.Min(currentShield, damageAfterProtection);
                currentShield -= damageToShield;
                damageAfterProtection -= damageToShield;
                UpdateShieldBar();

                if (currentShield <= 0 && shieldBreakSoundSource != null && !shieldBreakSoundSource.isPlaying)
                {
                    Debug.Log("Playing shield break sound");
                    shieldBreakSoundSource.Play();
                }
            }

            if (damageAfterProtection > 0)
            {
                currentEnergy -= damageAfterProtection;
                currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
                UpdateEnergyBar();

                if (damageSoundSource != null && !damageSoundSource.isPlaying)
                {
                    Debug.Log("Playing damage sound");
                    damageSoundSource.Play();
                }

                if (currentEnergy <= 0)
                {
                    Debug.Log("Player is out of energy (HP)!");
                }
            }

            Debug.Log("Player took " + damageAfterProtection + " damage after protection and shield.");

            isInvincible = true;
            invincibilityEndTime = Time.time + invincibilityDuration;
        }
    }
}
