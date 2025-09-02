using UnityEngine;
using System.Collections;

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

    [Header("Spawn Area")]
    public Vector3 areaSize = new Vector3(10, 0, 10); 
    public Vector3 areaCenter = Vector3.zero;         

    [Header("Timing")]
    public float minSpawnTime = 2f;
    public float maxSpawnTime = 5f;

    [Header("Limit")]
    public int maxSpawns = 10; // fixed number of spawns

    private int currentSpawnCount = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (currentSpawnCount < maxSpawns) // stop after reaching limit
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            TrySpawnTrash();
        }
    }

    void TrySpawnTrash()
    {
        if (currentSpawnCount >= maxSpawns) return;

        GameObject trashPrefab = GetRandomTrashPrefab();
        if (trashPrefab != null)
        {
            Vector3 randomPos = GetRandomPosition();
            Instantiate(trashPrefab, randomPos, Quaternion.identity);
            currentSpawnCount++; // count each spawn
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(-areaSize.y / 2, areaSize.y / 2),
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );
        return transform.position + areaCenter + randomOffset;
    }

    GameObject GetRandomTrashPrefab()
    {
        float total = 0f;
        foreach (TrashType type in trashTypes)
            total += type.spawnChance;

        float randomValue = Random.Range(0, total);
        float cumulative = 0f;

        foreach (TrashType type in trashTypes)
        {
            cumulative += type.spawnChance;
            if (randomValue <= cumulative)
                return type.prefab;
        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position + areaCenter, areaSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + areaCenter, areaSize);
    }
}
