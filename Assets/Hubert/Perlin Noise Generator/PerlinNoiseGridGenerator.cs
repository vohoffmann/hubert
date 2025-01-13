using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class PerlinNoiseGridGenerator : MonoBehaviour
{
    [Header("Grid Settings")] [SerializeField]
    private int gridWidth = 30;

    [SerializeField] private int gridHeight = 30;
    [SerializeField] private float noiseScale = 0.04f; // Controls the granularity of the Perlin noise

    [Header("Seed Settings")] [SerializeField]
    private float resourceNoiseScale = 0.05f; // Scale for resource noise

    [SerializeField] private float resourceThreshold = 0.8f; // Threshold for resource spawning

    [SerializeField] private int seed;
    [SerializeField] private bool useRandomSeed;

    [Header("Prefabs")] [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private GameObject treePrefab;
    [Header("Debugging")] [SerializeField] private bool showGizmos;
    [Header("NavMesh")] [SerializeField] private NavMeshSurface navMeshSurface;

    // private List<Vector3> grassTilePositions = new List<Vector3>(); // Track grass tile positions
    private Dictionary<Vector3, GameObject> grassTileDictionary = new();
    [SerializeField] private GameObject _level;

    private void Start()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue); // Generate a random seed
            Debug.Log($"Generated random seed: {seed}");
        }
        else
        {
            Debug.Log($"Using fixed seed: {seed}");
        }

        GenerateGrid(seed);
        GenerateResourcePatches(seed);

        // GenerateResourcesOnGrassTiles();

        // Build the NavMesh after generating the grid
        BuildNavMesh();
    }

    private void GenerateGrid(int seed)
    {
        Random.InitState(seed); // Set the random seed for deterministic results

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Generate Perlin noise value for the current grid position
                float perlinValue = Mathf.PerlinNoise((x + (float)seed) * noiseScale, (y + (float)seed) * noiseScale);
                // float perlinValue = Mathf.PerlinNoise(x * noiseScale + seed, y * noiseScale + seed);

                Debug.Log($"Perlin: {perlinValue})");

                // Determine terrain type based on Perlin noise value
                // int terrainType;
                // if (perlinValue < 0.13f)
                // {
                //     terrainType = 2; // Stone
                // }
                // else if (perlinValue < 0.25f)
                // {
                //     terrainType = 0; // Grass
                // }
                // else if (perlinValue < 0.45f)
                // {
                //     terrainType = 3; // Trees
                // }
                // else if (perlinValue < 0.75f)
                // {
                //     terrainType = 0; // Grass
                // }
                // else if (perlinValue < 1f)
                // {
                //     terrainType = 1; // Water
                // }
                // else
                // {
                //     terrainType = 0; // Grass
                // }

                // int terrainType;
                // if (perlinValue < 0.12f)
                // {
                //     terrainType = 2; // Stone
                // }
                // else if (perlinValue < 0.28f)
                // {
                //     terrainType = 0; // Grass
                // }
                // else if (perlinValue < 0.43f)
                // {
                //     terrainType = 1; // Water
                // }
                // else if (perlinValue < 0.70f)
                // {
                //     terrainType = 0; // Grass
                // }
                // else if (perlinValue < 1f)
                // {
                //     terrainType = 3; // Trees
                // }
                // else
                // {
                //     terrainType = 0; // Grass
                // }

                int terrainType;
                // create a walkable border around the map
                if (x == 0 || y == 0 || x == gridWidth - 1 || y == gridHeight - 1)
                {
                    terrainType = 0;
                    SpawnCell(x, y, terrainType);
                    continue;
                }

                if (perlinValue < 0.40f)
                {
                    terrainType = 3; // Trees
                }
                else if (perlinValue < 0.92f)
                {
                    terrainType = 0; // Grass
                }
                else if (perlinValue < 1f)
                {
                    terrainType = 0; // Stone
                }
                else
                {
                    terrainType = 0; // Grass
                }

                // Spawn the appropriate prefab
                SpawnCell(x, y, terrainType);
            }
        }
    }

    private void SpawnCell(int x, int y, int terrainType)
    {
        GameObject prefabToSpawn;

        switch (terrainType)
        {
            case 0:
                prefabToSpawn = grassPrefab;

                // Vector3 grassPosition = new Vector3(x * 10, 0, y * 10);

                // GameObject grassTile = Instantiate(prefabToSpawn, grassPosition, Quaternion.identity);
                // grassTileDictionary[grassPosition] = grassTile;
                //grassTilePositions.Add(new Vector3(x * 10, 0, y * 10));
                break;
            case 1:
                prefabToSpawn = waterPrefab;
                break;
            case 2:
                prefabToSpawn = stonePrefab;
                break;
            case 3:
                prefabToSpawn = treePrefab;
                break;
            default:
                Debug.LogError($"Invalid terrain type at ({x + 0.5}, {y + 0.5}): {terrainType}");
                return;
        }

        GameObject tile;
        Vector3 position = new Vector3(x , y , 0);
        tile = Instantiate(prefabToSpawn, position, Quaternion.identity);

        if (terrainType == 0)
        {
            grassTileDictionary[position] = tile;
        }

        tile.transform.SetParent(_level.transform);
    }

    private void BuildNavMesh()
    {
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface is not assigned!");
            return;
        }

        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh built successfully.");
    }

    // private void GenerateResourcesOnGrassTiles()
    // {
    //     foreach (Vector3 position in grassTilePositions)
    //     {
    //         // Adjust the position slightly above the ground to avoid overlap
    //         Vector3 resourcePosition = position + new Vector3(0, 0.5f, 0);

    //         // Random chance to spawn a resource (e.g., 50%)
    //         if (Random.value < 0.5f)
    //         {
    //             Instantiate(stonePrefab, resourcePosition, Quaternion.identity);
    //             Debug.Log($"Stone: {resourcePosition}.");
    //         }
    //     }

    //     Debug.Log($"Generated resources on {grassTilePositions.Count} grass tiles.");
    // }

    private void GenerateResourcePatches(int seed)
    {
        // Use a different seed offset for the resource noise map
        int resourceSeed = seed + 100;

        foreach (KeyValuePair<Vector3, GameObject> entry in grassTileDictionary)
        {
            Vector3 position = entry.Key;
            GameObject grassTile = entry.Value;

            // Generate Perlin noise value for this position
            float perlinValue = Mathf.PerlinNoise((position.x + resourceSeed) * resourceNoiseScale,
                (position.z + resourceSeed) * resourceNoiseScale);

            // Check if the noise value exceeds the resource threshold
            if (perlinValue > resourceThreshold)
            {
                // Replace the grass tile with a resource tile
                Destroy(grassTile);
                var tile = Instantiate(stonePrefab, position, Quaternion.identity);
                tile.transform.SetParent(_level.transform);
            }
        }

        Debug.Log($"Generated resource patches based on Perlin noise.");
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.green;

        // Draw the grid bounds in the scene view
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Gizmos.DrawWireCube(new Vector3(x, y, 0), new Vector3(1, 1, 0));
            }
        }
    }
}