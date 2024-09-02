using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance;

    public float maxEnergy = 100f;
    public float currentEnergy;
    public float maxShield = 50f;
    public float currentShield;
    public float shieldRecoveryRate = 5f;
    public float movementEnergyConsumptionRate = 5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally: Use DontDestroyOnLoad here if you want to test persistence behavior
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
