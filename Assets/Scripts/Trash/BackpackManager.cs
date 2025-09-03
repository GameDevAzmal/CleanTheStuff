using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BackpackManager : MonoBehaviour
{
    public static BackpackManager Instance;

    [Header("Backpack Settings")]
    public int numberOfSlots = 6; // Number of inventory slots
    
    [Header("UI References")]
    public TextMeshProUGUI notificationText;
    public float notificationDuration = 2f;

    private InventorySlot[] inventorySlots;
    private Coroutine notificationCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeInventory()
    {
        inventorySlots = new InventorySlot[numberOfSlots];
        for (int i = 0; i < numberOfSlots; i++)
        {
            inventorySlots[i] = new InventorySlot();
        }
    }

    // Try to add item to backpack
    public bool TryAddItem(string itemName, int cashValue, int stackLimit)
    {
        int remainingToAdd = 1; // Usually adding 1 item at a time

        // First, try to add to existing slots with same item type
        for (int i = 0; i < inventorySlots.Length && remainingToAdd > 0; i++)
        {
            if (!inventorySlots[i].isEmpty && inventorySlots[i].itemType == itemName)
            {
                int added = inventorySlots[i].AddItems(itemName, remainingToAdd, stackLimit);
                remainingToAdd -= added;
            }
        }

        // If still have items to add, try empty slots
        for (int i = 0; i < inventorySlots.Length && remainingToAdd > 0; i++)
        {
            if (inventorySlots[i].isEmpty)
            {
                int added = inventorySlots[i].AddItems(itemName, remainingToAdd, stackLimit);
                remainingToAdd -= added;
            }
        }

        // Return true if we managed to add the item
        if (remainingToAdd == 0)
        {
            ShowNotification($"Collected {itemName}");
            return true;
        }
        else
        {
            ShowNotification("Backpack Full!");
            return false;
        }
    }

    // Remove items from backpack (for depositing)
    public int RemoveItems(string itemName, int amount)
    {
        int totalRemoved = 0;
        int stillNeedToRemove = amount;

        // Remove from slots that have this item
        for (int i = 0; i < inventorySlots.Length && stillNeedToRemove > 0; i++)
        {
            if (!inventorySlots[i].isEmpty && inventorySlots[i].itemType == itemName)
            {
                int removed = inventorySlots[i].RemoveItems(stillNeedToRemove);
                totalRemoved += removed;
                stillNeedToRemove -= removed;
            }
        }

        return totalRemoved;
    }

    // Get count of specific item in backpack
    public int GetItemCount(string itemName)
    {
        int count = 0;
        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.isEmpty && slot.itemType == itemName)
            {
                count += slot.currentCount;
            }
        }
        return count;
    }

    // Check if backpack is completely full - FIXED VERSION
    public bool IsBackpackFull()
    {
        // First check if we have any empty slots
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.isEmpty)
                return false; // Found an empty slot, so not full
        }
        
        // If no empty slots, check if any slot has available space
        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.isFull)
                return false; // Found a slot with space, so not full
        }
        
        return true; // All slots are full
    }

    // Get all unique items in backpack
    public Dictionary<string, int> GetAllItems()
    {
        Dictionary<string, int> items = new Dictionary<string, int>();
        
        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.isEmpty)
            {
                if (items.ContainsKey(slot.itemType))
                    items[slot.itemType] += slot.currentCount;
                else
                    items[slot.itemType] = slot.currentCount;
            }
        }
        
        return items;
    }

    // Get inventory slots for UI display
    public InventorySlot[] GetInventorySlots()
    {
        return inventorySlots;
    }

    // Show notification message
    public void ShowNotification(string message)
    {
        if (notificationText == null) return;

        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(NotificationRoutine(message));
    }

    private IEnumerator NotificationRoutine(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(notificationDuration);

        notificationText.gameObject.SetActive(false);
    }

    // Clear entire backpack
    public void ClearBackpack()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            slot.Clear();
        }
    }

    // Add debug method to check backpack state
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugBackpackState()
    {
        Debug.Log($"Backpack State - Total Slots: {numberOfSlots}");
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i];
            Debug.Log($"Slot {i}: {slot.GetDisplayText()} | IsEmpty: {slot.isEmpty} | IsFull: {slot.isFull}");
        }
        Debug.Log($"IsBackpackFull(): {IsBackpackFull()}");
    }
}