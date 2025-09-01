using UnityEngine;
using System.Collections;

public class TrashSpawner : MonoBehaviour
{
    [System.Serializable]
    public class TrashType
    {
        public GameObject prefab;  
        public float spawnChance;   
    }

    [System.Serializable]
    public class SpawnPoint
    {
        public Transform point;     
        [HideInInspector] public bool isOccupied = false; 
    }

    [Header("Trash Settings")]
    public TrashType[] trashTypes;

    [Header("Spawn Points")]
    public SpawnPoint[] spawnPoints;

    [Header("Timing")]
    public float minSpawnTime = 2f;
    public float maxSpawnTime = 5f;

    void Start()
    {
       
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            TrySpawnTrash();
        }
    }

    void TrySpawnTrash()
    {
        if (spawnPoints.Length == 0 || trashTypes.Length == 0) return;

       
        int index = Random.Range(0, spawnPoints.Length);
        SpawnPoint sp = spawnPoints[index];

        if (!sp.isOccupied) 
        {
            GameObject trashPrefab = GetRandomTrashPrefab();
            if (trashPrefab != null)
            {
                GameObject spawnedTrash = Instantiate(trashPrefab, sp.point.position, sp.point.rotation);
                sp.isOccupied = true;

               
                Trash trashComp = spawnedTrash.GetComponent<Trash>();
                if (trashComp != null)
                {
                    trashComp.onTrashDestroyed += () => sp.isOccupied = false;
                }
                else
                {
                   
                    Destroy(spawnedTrash, 10f); 
                    sp.isOccupied = false;
                }
            }
        }
    }

    GameObject GetRandomTrashPrefab()
    {
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
}
