using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    // Dictionary to store trash counts by type
    public Dictionary<string, int> trashCounts = new Dictionary<string, int>();

    // Max backpack capacity
    public int backpackCapacity = 10;
    private int currentBackpackCount = 0;

    // Called when trash is collected
    public void CollectTrash(GameObject trash)
    {
        string trashType = trash.name; // Or you can use tag/layer if better

        // Check backpack capacity
        if (currentBackpackCount >= backpackCapacity)
        {
            Debug.Log("Backpack full!");
            return;
        }

        // Add or update dictionary
        if (trashCounts.ContainsKey(trashType))
        {
            trashCounts[trashType]++;
        }
        else
        {
            trashCounts[trashType] = 1;
        }

        currentBackpackCount++;

        Debug.Log($"Collected {trashType}, total = {trashCounts[trashType]}");
        Debug.Log($"Total trash: {currentBackpackCount}");
        Destroy(trash); // remove the object from the scene
    }

    // Example: check how much trash of one type
    public int GetTrashCount(string type)
    {
        return trashCounts.ContainsKey(type) ? trashCounts[type] : 0;
    }

    // Example: get total trash carried
    public int GetTotalTrash()
    {
        return currentBackpackCount;
    }
}
