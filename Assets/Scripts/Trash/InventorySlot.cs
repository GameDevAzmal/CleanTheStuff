using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public string itemType = ""; // Name of trash type (e.g., "Bottle", "Can")
    public int currentCount = 0; // How many items currently in this slot
    public int maxCount = 0; // Maximum items this slot can hold for this item type
    public bool isEmpty => currentCount == 0;
    public bool isFull => currentCount >= maxCount;
    public int availableSpace => maxCount - currentCount;

    public InventorySlot()
    {
        Clear();
    }

    // Try to add items to this slot
    public int AddItems(string trashType, int amount, int stackLimit)
    {
        // If slot is empty, initialize it with this item type
        if (isEmpty)
        {
            itemType = trashType;
            maxCount = stackLimit;
        }

        // Can only add if same item type
        if (itemType != trashType)
            return 0; // Couldn't add any

        // Calculate how many we can actually add
        int canAdd = Mathf.Min(amount, availableSpace);
        currentCount += canAdd;

        return canAdd; // Return how many were actually added
    }

    // Remove items from this slot
    public int RemoveItems(int amount)
    {
        if (isEmpty)
            return 0;

        int canRemove = Mathf.Min(amount, currentCount);
        currentCount -= canRemove;

        // If empty, clear the slot
        if (currentCount <= 0)
            Clear();

        return canRemove; // Return how many were actually removed
    }

    // Clear the slot completely
    public void Clear()
    {
        itemType = "";
        currentCount = 0;
        maxCount = 0;
    }

    // Get display text for UI
    public string GetDisplayText()
    {
        if (isEmpty)
            return "Empty";
        
        return $"{itemType}: {currentCount}/{maxCount}";
    }
}