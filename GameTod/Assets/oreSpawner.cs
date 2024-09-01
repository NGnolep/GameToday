using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreAndSpikeSpawner : MonoBehaviour
{
    public List<GameObject> ores;
    public GameObject spikePrefab;
    public int maxOresPerLevel = 20;
    public int oresPerStage = 3;
    public int levelsPerStage = 10;
    public int minSpikesPerLevel = 10;
    public int maxSpikesPerLevel = 20;
    public float minDistanceBetweenOres = 3.0f;
    public float minDistanceBetweenSpikes = 3.0f;
    public float minDistanceBetweenOreAndSpike = 3.0f;

    private List<Terrain> childTerrains = new List<Terrain>();
    private List<Vector3> orePositions = new List<Vector3>();
    private List<Vector3> spikePositions = new List<Vector3>();
    private float minSpawnHeight = 0f;
    private float maxSpawnHeight = 1f;
    private GameObject[] spawnedOres;
    private GameObject[] spawnedSpikes;

    public void Initialize(Terrain parentTerrain)
    {
        // Check and add the parent terrain to the list
        if (parentTerrain != null)
        {
            childTerrains.Clear(); // Clear existing terrains before adding
            childTerrains.Add(parentTerrain);
            Debug.Log($"Added parent terrain: {parentTerrain.name}");
        }
        else
        {
            Debug.LogError("Parent terrain is missing a Terrain component!");
            return;
        }

        // Ensure the parent terrain has children
        if (parentTerrain.transform.childCount == 0)
        {
            Debug.LogError("No child terrains found under the parent terrain!");
            return;
        }

        // Find all child terrains and add them to the list
        foreach (Transform child in parentTerrain.transform)
        {
            Terrain terrain = child.GetComponent<Terrain>();
            if (terrain != null)
            {
                childTerrains.Add(terrain);
                Debug.Log($"Added child terrain: {terrain.name}");
            }
            else
            {
                Debug.LogWarning($"Child {child.name} is missing a Terrain component!");
            }
        }

        if (childTerrains.Count == 0)
        {
            Debug.LogError("No valid terrains found!");
            return;
        }

        // Clear previously spawned objects if transitioning levels
        if (LevelManager.Instance.IsTransitioningToNextLevel)
        {
            ClearSpawnedObjects();
            LevelManager.Instance.IsTransitioningToNextLevel = false; // Reset transition flag
        }

        // Start spawning ores and spikes
        StartCoroutine(SpawnOresAndSpikes());
    }

    private IEnumerator SpawnOresAndSpikes()
    {
        // Ensure terrains are present
        if (childTerrains.Count == 0)
        {
            Debug.LogWarning("No terrains available for spawning.");
            yield break;
        }

        // Spawn ores
        yield return StartCoroutine(SpawnOres());

        // Spawn spikes
        yield return StartCoroutine(SpawnSpikes());
    }

    private IEnumerator SpawnOres()
    {
        int oresToInclude = GetOresToIncludeBasedOnLevel();
        int totalOresToSpawn = Mathf.Min(maxOresPerLevel, oresToInclude * 10);
        int oreCountPerType = Mathf.Max(10, totalOresToSpawn / oresToInclude);

        for (int i = 0; i < oresToInclude; i++)
        {
            for (int j = 0; j < oreCountPerType; j++)
            {
                Vector3 randomPosition = GetValidOrePosition();

                if (randomPosition != Vector3.zero)
                {
                    GameObject ore = Instantiate(ores[i], randomPosition, Quaternion.identity);
                    orePositions.Add(randomPosition);
                    spawnedOres = AddToArray(spawnedOres, ore); // Track spawned ores
                }

                yield return null;
            }
        }

        int remainingOresToSpawn = totalOresToSpawn - (oreCountPerType * oresToInclude);
        for (int i = 0; i < remainingOresToSpawn; i++)
        {
            Vector3 randomPosition = GetValidOrePosition();

            if (randomPosition != Vector3.zero)
            {
                int oreIndex = Random.Range(0, oresToInclude);
                GameObject ore = Instantiate(ores[oreIndex], randomPosition, Quaternion.identity);
                orePositions.Add(randomPosition);
                spawnedOres = AddToArray(spawnedOres, ore); // Track spawned ores
            }

            yield return null;
        }
    }

    private IEnumerator SpawnSpikes()
    {
        int numSpikes = Random.Range(minSpikesPerLevel, maxSpikesPerLevel + 1);

        for (int i = 0; i < numSpikes; i++)
        {
            Vector3 randomPosition = GetValidSpikePosition();

            if (randomPosition != Vector3.zero)
            {
                GameObject spike = Instantiate(spikePrefab, randomPosition, Quaternion.identity);
                spikePositions.Add(randomPosition);
                spawnedSpikes = AddToArray(spawnedSpikes, spike); // Track spawned spikes
            }

            yield return null;
        }
    }

    private void ClearSpawnedObjects()
    {
        // Clear previously spawned ores and spikes
        if (spawnedOres != null)
        {
            foreach (GameObject ore in spawnedOres)
            {
                if (ore != null)
                    Destroy(ore);
            }
        }

        if (spawnedSpikes != null)
        {
            foreach (GameObject spike in spawnedSpikes)
            {
                if (spike != null)
                    Destroy(spike);
            }
        }

        // Clear lists
        orePositions.Clear();
        spikePositions.Clear();
    }

    private GameObject[] AddToArray(GameObject[] array, GameObject item)
    {
        if (array == null)
        {
            return new GameObject[] { item };
        }
        else
        {
            GameObject[] newArray = new GameObject[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }
            newArray[array.Length] = item;
            return newArray;
        }
    }

    Vector3 GetRandomPositionOnTerrain(Terrain terrain)
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

            Terrain selectedTerrain = childTerrains[Random.Range(0, childTerrains.Count)];
            position = GetRandomPositionOnTerrain(selectedTerrain);

            float terrainHeight = selectedTerrain.SampleHeight(position) + selectedTerrain.transform.position.y;
            if (terrainHeight < minSpawnHeight || terrainHeight > maxSpawnHeight)
            {
                validPositionFound = false;
                continue;
            }

            foreach (var orePosition in orePositions)
            {
                if (Vector3.Distance(position, orePosition) < minDistanceBetweenOres)
                {
                    validPositionFound = false;
                    break;
                }
            }

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
        int attempts = 0;

        do
        {
            validPositionFound = true;

            Terrain selectedTerrain = childTerrains[Random.Range(0, childTerrains.Count)];
            position = GetRandomPositionOnTerrain(selectedTerrain);

            float terrainHeight = selectedTerrain.SampleHeight(position) + selectedTerrain.transform.position.y;
            if (terrainHeight < minSpawnHeight || terrainHeight > maxSpawnHeight)
            {
                validPositionFound = false;
                continue;
            }

            foreach (var spikePosition in spikePositions)
            {
                if (Vector3.Distance(position, spikePosition) < minDistanceBetweenSpikes)
                {
                    validPositionFound = false;
                    break;
                }
            }

            foreach (var orePosition in orePositions)
            {
                if (Vector3.Distance(position, orePosition) < minDistanceBetweenOreAndSpike)
                {
                    validPositionFound = false;
                    break;
                }
            }

            attempts++;
            if (attempts > 50)
            {
                Debug.LogWarning("Could not find a valid position for spike after 50 attempts.");
                return Vector3.zero;
            }

        } while (!validPositionFound);

        return position;
    }

    private int GetOresToIncludeBasedOnLevel()
    {
        int stageNumber = (LevelManager.Instance.CurrentLevel - 1) / levelsPerStage + 1;

        if (stageNumber <= 1)
            return Mathf.Min(3, ores.Count); // First stage level 1-10
        else if (stageNumber <= 2)
            return Mathf.Min(6, ores.Count); // Second stage level 11-20
        else
            return ores.Count; // Final stage level 21-25
    }
}
