using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject terrain1Prefab; // Assign the first terrain prefab in the inspector
    public GameObject terrain2Prefab; // Assign the second terrain prefab in the inspector
    public GameObject playerPrefab; // Assign the player prefab in the inspector
    public GameObject oreAndSpikeSpawnerPrefab; // Assign the OreAndSpikeSpawner prefab in the inspector

    private GameObject selectedTerrain;

    void Start()
    {
        // Randomly select one of the terrains
        int randomTerrain = Random.Range(0, 2);

        if (randomTerrain == 0)
        {
            selectedTerrain = Instantiate(terrain1Prefab);
        }
        else
        {
            selectedTerrain = Instantiate(terrain2Prefab);
        }

        // Find the existing staircase in the terrain
        GameObject existingStaircase = GameObject.FindWithTag("Staircase");

        if (existingStaircase != null)
        {
            // Place the player on the staircase
            GameObject player = SpawnPlayerOnStaircase(existingStaircase);

            // Find the CamController and assign the spawned player to it
            CamController camController = FindObjectOfType<CamController>();
            if (camController != null)
            {
                camController.player = player;
            }
        }
        else
        {
            Debug.LogWarning("Staircase not found! Ensure the staircase has the 'Staircase' tag.");
        }

        // Spawn the OreAndSpikeSpawner and initialize it with the terrain
        GameObject oreAndSpikeSpawner = Instantiate(oreAndSpikeSpawnerPrefab);
        OreAndSpikeSpawner spawnerScript = oreAndSpikeSpawner.GetComponent<OreAndSpikeSpawner>();
        if (spawnerScript != null)
        {
            spawnerScript.Initialize(selectedTerrain.GetComponent<Terrain>());
        }
    }

    GameObject SpawnPlayerOnStaircase(GameObject staircase)
    {
        Vector3 playerPosition = staircase.transform.position + new Vector3(0, 1, 0); // Adjust for correct placement
        GameObject player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        // Ensure the player has the correct components
        player.GetComponent<Rigidbody>().useGravity = true;

        return player;
    }
}
