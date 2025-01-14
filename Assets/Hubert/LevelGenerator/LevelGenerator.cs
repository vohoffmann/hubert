using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Hubert.LevelGenerator
{
    public class LevelGenerator : MonoBehaviour
    {
        [Header("Grid Settings")] [SerializeField]
        private int gridWidth = 30;

        [SerializeField] private int gridHeight = 30;
        [SerializeField] private float noiseScale = 0.04f;

        [Header("Seed Settings")] [SerializeField]
        private float resourceNoiseScale = 0.05f;

        [SerializeField] private float resourceThreshold = 0.8f;
        [SerializeField] private int seed;
        [SerializeField] private bool useRandomSeed;

        [Header("Prefabs")] [SerializeField] private GameObject grassPrefab;
        [SerializeField] private GameObject waterPrefab;
        [SerializeField] private GameObject stonePrefab;
        [SerializeField] private GameObject treePrefab;
        [Header("Debugging")] [SerializeField] private bool showGizmos;
        [Header("NavMesh")] [SerializeField] private NavMeshSurface navMeshSurface;

        private readonly Dictionary<Vector3, GameObject> grassTileDictionary = new();
        [SerializeField] private GameObject _level;

        private void Start()
        {
            if (useRandomSeed)
            {
                seed = Random.Range(int.MinValue, int.MaxValue);
                Debug.Log($"Generated random seed: {seed}");
            }
            else
            {
                Debug.Log($"Using fixed seed: {seed}");
            }

            GenerateGrid(seed);
            GenerateResourcePatches(seed);
            BuildNavMesh();
        }

        private void GenerateGrid(int pSeed)
        {
            Random.InitState(pSeed);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    float perlinValue =
                        Mathf.PerlinNoise((x + (float)pSeed) * noiseScale, (y + (float)pSeed) * noiseScale);

                    // Debug.Log($"Perlin: {perlinValue})");

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
                    Debug.LogError($"Invalid terrain type at ({x}, {y}): {terrainType}");
                    return;
            }

            Vector3 position = new Vector3(x, 0, y);
            var tile = Instantiate(prefabToSpawn, position, Quaternion.identity);

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

        private void GenerateResourcePatches(int pSeed)
        {
            // Use a different seed offset for the resource noise map
            int resourceSeed = pSeed + 100;

            foreach (var (position, grassTile) in grassTileDictionary)
            {
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

            Debug.Log("Generated resource patches based on Perlin noise.");
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            Gizmos.color = Color.gray;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    Gizmos.DrawWireCube(new Vector3(x, 0, z), new Vector3(1, 0, 1));
                }
            }
        }
    }
}