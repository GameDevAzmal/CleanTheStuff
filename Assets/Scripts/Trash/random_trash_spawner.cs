using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrashSpawnerRandom : MonoBehaviour
{
    [System.Serializable]
    public class TrashType
    {
        public GameObject prefab;
        public float spawnChance;
    }

    [Header("Trash Settings")]
    public TrashType[] trashTypes;
    public int maxTrashCount = 50; // Maximum trash objects at once
    
    [Header("Spawn Area")]
    public Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f); // Area dimensions
    public LayerMask groundLayer = 1; // What layer to spawn on
    public float spawnHeight = 10f; // Height to raycast from
    
    [Header("Timing")]
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;
    
    [Header("Validation")]
    public float minDistanceBetweenTrash = 2f; // Minimum distance between trash objects
    public int maxSpawnAttempts = 10; // Max attempts to find valid spawn position

    private List<GameObject> spawnedTrash = new List<GameObject>(); // Track spawned trash

    void Start()
    {
        // Start the random spawning process
        StartCoroutine(RandomSpawnLoop());
    }

    IEnumerator RandomSpawnLoop()
    {
        while (true)
        {
            // Wait random time between spawns
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            
            // Only spawn if under max limit
            if (spawnedTrash.Count < maxTrashCount)
            {
                TrySpawnRandomTrash();
            }
        }
    }

    void TrySpawnRandomTrash()
    {
        if (trashTypes.Length == 0) return;

        Vector3 spawnPosition = Vector3.zero; // Initialize to prevent compiler error
        bool validPositionFound = false;

        // Try multiple times to find a valid spawn position
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            // Generate random position within spawn area
            Vector3 randomPoint = GetRandomPointInArea();
            
            // Try to find ground position using raycast
            if (TryFindGroundPosition(randomPoint, out spawnPosition))
            {
                // Check if position is far enough from other trash
                if (IsPositionValid(spawnPosition))
                {
                    validPositionFound = true;
                    break;
                }
            }
        }

        // Spawn trash if valid position found
        if (validPositionFound)
        {
            GameObject trashPrefab = GetRandomTrashPrefab();
            if (trashPrefab != null)
            {
                SpawnTrashAtPosition(trashPrefab, spawnPosition);
            }
        }
    }

    Vector3 GetRandomPointInArea()
    {
        // Generate random position within the defined area
        float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float randomZ = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);
        
        // Use spawner's position as center point
        return transform.position + new Vector3(randomX, spawnHeight, randomZ);
    }

    bool TryFindGroundPosition(Vector3 startPoint, out Vector3 groundPosition)
    {
        // Raycast downward to find ground
        RaycastHit hit;
        if (Physics.Raycast(startPoint, Vector3.down, out hit, spawnHeight + 5f, groundLayer))
        {
            groundPosition = hit.point;
            return true;
        }
        
        groundPosition = Vector3.zero;
        return false;
    }

    bool IsPositionValid(Vector3 position)
    {
        // Check distance from all existing trash
        foreach (GameObject trash in spawnedTrash)
        {
            if (trash != null && Vector3.Distance(position, trash.transform.position) < minDistanceBetweenTrash)
            {
                return false;
            }
        }
        return true;
    }

    void SpawnTrashAtPosition(GameObject trashPrefab, Vector3 position)
    {
        // Add random rotation for variety
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        
        // Spawn the trash
        GameObject spawnedObject = Instantiate(trashPrefab, position, randomRotation);
        spawnedTrash.Add(spawnedObject);

        // Setup cleanup callback
        Trash trashComponent = spawnedObject.GetComponent<Trash>();
        if (trashComponent != null)
        {
            trashComponent.onTrashDestroyed += () => RemoveTrashFromList(spawnedObject);
        }
        else
        {
            // Fallback: auto-cleanup after time
            StartCoroutine(CleanupTrashAfterTime(spawnedObject, 30f));
        }
    }

    void RemoveTrashFromList(GameObject trash)
    {
        // Remove destroyed trash from tracking list
        spawnedTrash.Remove(trash);
    }

    IEnumerator CleanupTrashAfterTime(GameObject trash, float time)
    {
        yield return new WaitForSeconds(time);
        if (trash != null)
        {
            RemoveTrashFromList(trash);
            Destroy(trash);
        }
    }

    GameObject GetRandomTrashPrefab()
    {
        // Weighted random selection (same as original spawner)
        float total = 0f;
        foreach (TrashType type in trashTypes)
        {
            total += type.spawnChance;
        }

        float randomValue = Random.Range(0, total);
        float cumulative = 0f;

        foreach (TrashType type in trashTypes)
        {
            cumulative += type.spawnChance;
            if (randomValue <= cumulative)
            {
                return type.prefab;
            }
        }
        return null;
    }

    // Cleanup null references from the list
    void Update()
    {
        // Remove any destroyed trash objects from list (cleanup)
        spawnedTrash.RemoveAll(trash => trash == null);
    }

    // Visual helper in Scene view
    void OnDrawGizmosSelected()
    {
        // Draw spawn area bounds in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
        
        // Draw spawn height
        Gizmos.color = Color.red;
        Vector3 heightPos = transform.position + Vector3.up * spawnHeight;
        Gizmos.DrawWireCube(heightPos, spawnAreaSize);
    }
}