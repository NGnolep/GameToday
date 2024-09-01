using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreAndSpikeSpawner : MonoBehaviour
{
    public List<GameObject> ores;
    public GameObject spikePrefab;
    public int currentLevel = 1;
    public int maxOresPerLevel = 20;
    public int oresPerStage = 3;
    public int levelsPerStage = 10;
    public int minSpikesPerLevel = 10;
    public int maxSpikesPerLevel = 20;
    public float minDistanceBetweenOres = 3.0f; // Minimum distance between ores
    public float minDistanceBetweenSpikes = 3.0f; // Minimum distance between spikes
    public float minDistanceBetweenOreAndSpike = 3.0f; // Minimum distance between ores and spikes

    private Terrain terrain;
    private List<Vector3> orePositions = new List<Vector3>(); // Track spawned ore positions
    private List<Vector3> spikePositions = new List<Vector3>(); // Track spawned spike positions
    private float minSpawnHeight = 0f; // Minimum height for spawning
    private float maxSpawnHeight = 1f; // Maximum height for spawning

    public void Initialize(Terrain spawnedTerrain)
    {
        terrain = spawnedTerrain;
        StartCoroutine(SpawnOresAndSpikes());
    }

    private IEnumerator SpawnOresAndSpikes()
    {
        // Spawn ores
        yield return StartCoroutine(SpawnOres());

        // Spawn spikes
        yield return StartCoroutine(SpawnSpikes());
    }

    private IEnumerator SpawnOres()
    {
        if (terrain == null)
        {
            Debug.LogWarning("Terrain not assigned! Ensure that the terrain is properly initialized.");
            yield break;
        }

        int oresToInclude = GetOresToIncludeBasedOnLevel();
        int totalOresToSpawn = Mathf.Min(maxOresPerLevel, oresToInclude * 10);
        int oreCountPerType = Mathf.Max(10, totalOresToSpawn / oresToInclude);

        Debug.Log($"Spawning {totalOresToSpawn} ores. Each ore type will spawn at least {oreCountPerType} times.");

        // Ensure each ore type spawns at least the minimum amount
        for (int i = 0; i < oresToInclude; i++)
        {
            for (int j = 0; j < oreCountPerType; j++)
            {
                Vector3 randomPosition = GetValidOrePosition();

                if (randomPosition != Vector3.zero) // Ensure a valid position is found
                {
                    Instantiate(ores[i], randomPosition, Quaternion.identity);
                    orePositions.Add(randomPosition);
                    Debug.Log($"Ore of type {i} spawned at: {randomPosition}");
                }
                else
                {
                    Debug.LogWarning("Failed to find a valid position for ore.");
                }

                yield return null; // Wait until the next frame to prevent freezing
            }
        }

        // If there are still more ores to spawn, continue to spawn remaining ores
        int remainingOresToSpawn = totalOresToSpawn - (oreCountPerType * oresToInclude);
        for (int i = 0; i < remainingOresToSpawn; i++)
        {
            Vector3 randomPosition = GetValidOrePosition();

            if (randomPosition != Vector3.zero) // Ensure a valid position is found
            {
                int oreIndex = Random.Range(0, oresToInclude);
                Instantiate(ores[oreIndex], randomPosition, Quaternion.identity);
                orePositions.Add(randomPosition);
                Debug.Log($"Additional ore of type {oreIndex} spawned at: {randomPosition}");
            }
            else
            {
                Debug.LogWarning("Failed to find a valid position for additional ore.");
            }

            yield return null; // Wait until the next frame to prevent freezing
        }
    }

    private IEnumerator SpawnSpikes()
    {
        if (terrain == null)
        {
            Debug.LogWarning("Terrain not assigned! Ensure that the terrain is properly initialized.");
            yield break;
        }

        int numSpikes = Random.Range(minSpikesPerLevel, maxSpikesPerLevel + 1);

        Debug.Log($"Spawning {numSpikes} spikes.");

        for (int i = 0; i < numSpikes; i++)
        {
            Vector3 randomPosition = GetValidSpikePosition();

            if (randomPosition != Vector3.zero) // Check if a valid position was found
            {
                // Spawn the spike
                Instantiate(spikePrefab, randomPosition, Quaternion.identity);

                // Store the position to prevent overlapping with other spikes and ores
                spikePositions.Add(randomPosition);

                Debug.Log($"Spike spawned at: {randomPosition}");
            }
            else
            {
                Debug.LogWarning("Failed to find a valid position for spike after several attempts.");
            }

            yield return null; // Wait until the next frame to prevent freezing
        }
    }

    Vector3 GetRandomPositionOnTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;
        float randomX = Random.Range(terrainPosition.x, terrainPosition.x + terrainData.size.x);
        float randomZ = Random.Range(terrainPosition.z, terrainPosition.z + terrainData.size.z);
        float terrainHeight = terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrainPosition.y;

        return new Vector3(randomX, terrainHeight, randomZ);
    }

    Vector3 GetValidOrePosition()
    {
        Vector3 position;
        bool validPositionFound;
        int attempts = 0;

        do
        {
            validPositionFound = true;
            position = GetRandomPositionOnTerrain();

            // Ensure the height is within the specified range
            float terrainHeight = terrain.SampleHeight(position) + terrain.transform.position.y;
            if (terrainHeight < minSpawnHeight || terrainHeight > maxSpawnHeight)
            {
                validPositionFound = false;
                continue;
            }

            // Check the distance from all previously spawned ores
            foreach (var orePosition in orePositions)
            {
                if (Vector3.Distance(position, orePosition) < minDistanceBetweenOres)
                {
                    validPositionFound = false;
                    break;
                }
            }

            // Check the distance from all previously spawned spikes
            foreach (var spikePosition in spikePositions)
            {
                if (Vector3.Distance(position, spikePosition) < minDistanceBetweenOreAndSpike)
                {
                    validPositionFound = false;
                    break;
                }
            }

            attempts++;
            if (attempts > 50)
            {
                Debug.LogWarning("Could not find a valid position for ore after 50 attempts.");
                return Vector3.zero;
            }

        } while (!validPositionFound);

        return position;
    }

    Vector3 GetValidSpikePosition()
    {
        Vector3 position;
        bool validPositionFound;
        int attempts = 0;  // Counter to avoid infinite loops

        do
        {
            validPositionFound = true;
            position = GetRandomPositionOnTerrain();

            // Ensure the height is within the specified range
            float terrainHeight = terrain.SampleHeight(position) + terrain.transform.position.y;
            if (terrainHeight < minSpawnHeight || terrainHeight > maxSpawnHeight)
            {
                validPositionFound = false;
                continue;
            }

            // Check the distance from all previously spawned spikes
            foreach (var spikePosition in spikePositions)
            {
                if (Vector3.Distance(position, spikePosition) < minDistanceBetweenSpikes)
                {
                    validPositionFound = false;
                    break;
                }
            }

            // Check the distance from all previously spawned ores
            foreach (var orePosition in orePositions)
            {
                if (Vector3.Distance(position, orePosition) < minDistanceBetweenOreAndSpike)
                {
                    validPositionFound = false;
                    break;
                }
            }

            attempts++;
            if (attempts > 50)  // Limit attempts to find a valid position
            {
                Debug.LogWarning("Could not find a valid position for spike after 50 attempts.");
                return Vector3.zero;
            }

        } while (!validPositionFound);

        return position;
    }

    private int GetOresToIncludeBasedOnLevel()
    {
        int stageNumber = (currentLevel - 1) / levelsPerStage + 1;

        // Determine how many ore types should be included based on the level stage
        if (stageNumber <= 1)
            return Mathf.Min(3, ores.Count); // First stage level 1-10
        else if (stageNumber <= 2)
            return Mathf.Min(6, ores.Count); // Second stage level 11-20
        else
            return ores.Count; // Final stage level 21-25
    }
}
