using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<GameObject> collectedOres = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddOre(GameObject ore)
    {
        collectedOres.Add(ore);
        Debug.Log($"{ore.name} added to inventory.");
    }

    public bool HasOre(GameObject ore)
    {
        return collectedOres.Contains(ore);
    }
}
