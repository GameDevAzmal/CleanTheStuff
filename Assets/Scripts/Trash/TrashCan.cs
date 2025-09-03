using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TrashCan : MonoBehaviour
{
    [Header("Trash Can Settings")]
    public bool acceptsRecyclable = true; // true = recyclable bin, false = non-recyclable bin
    public KeyCode interactKey = KeyCode.E;
    public float depositTime = 1f; // Time to deposit items
    
    [Header("UI References")]
    public Slider depositSlider;
    public GameObject interactPrompt; // "Press E to deposit" UI

    [Header("Debug")]
    public bool showDebugLogs = true;

    private bool playerInRange = false;
    private bool isDepositing = false;
    private Coroutine depositCoroutine;

    void Start()
    {
        if (depositSlider != null)
            depositSlider.gameObject.SetActive(false);
            
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (showDebugLogs)
            Debug.Log($"TrashCan initialized - Accepts Recyclable: {acceptsRecyclable}");
    }

    void Update()
    {
        if (playerInRange && !isDepositing)
        {
            // Check if player has items that can be deposited in this bin
            if (HasDepositableItems())
            {
                if (interactPrompt != null)
                    interactPrompt.SetActive(true);

                if (Input.GetKeyDown(interactKey))
                {
                    StartDepositing();
                }
            }
            else
            {
                if (interactPrompt != null)
                    interactPrompt.SetActive(false);
            }
        }

        // Handle deposit progress
        if (isDepositing && Input.GetKey(interactKey))
        {
            // Continue depositing (handled in coroutine)
        }
        else if (isDepositing && !Input.GetKey(interactKey))
        {
            // Stop depositing if key released
            StopDepositing();
        }
    }

    bool HasDepositableItems()
    {
        var backpack = BackpackManager.Instance;
        if (backpack == null) 
        {
            if (showDebugLogs) Debug.Log("BackpackManager.Instance is null");
            return false;
        }

        // Get all items from backpack
        var allItems = backpack.GetAllItems();
        
        if (showDebugLogs)
            Debug.Log($"Checking {allItems.Count} item types in backpack");

        foreach (var item in allItems)
        {
            if (showDebugLogs)
                Debug.Log($"Checking item: {item.Key} (Count: {item.Value})");
                
            if (IsItemCompatible(item.Key) && item.Value > 0)
            {
                if (showDebugLogs)
                    Debug.Log($"Found compatible item: {item.Key}");
                return true;
            }
        }

        if (showDebugLogs)
            Debug.Log("No compatible items found");
        return false;
    }

    bool IsItemCompatible(string itemName)
    {
        if (showDebugLogs)
            Debug.Log($"Checking compatibility for: '{itemName}' with recyclable bin: {acceptsRecyclable}");

        // We need to check the actual recyclable property from the trash items
        // Since we only have the item name, we need to track this information
        
        // Check if this item type is recyclable based on the trash data
        bool itemIsRecyclable = GetItemRecyclableStatus(itemName);
        
        if (showDebugLogs)
            Debug.Log($"Item '{itemName}' is recyclable: {itemIsRecyclable}, Bin accepts recyclable: {acceptsRecyclable}");

        // Match the item type with the bin type
        if (acceptsRecyclable && itemIsRecyclable) return true;
        if (!acceptsRecyclable && !itemIsRecyclable) return true;

        return false;
    }

    // Helper method to determine if an item type is recyclable
    // This should match the isRecyclable setting from your Trash.cs prefabs
    bool GetItemRecyclableStatus(string itemName)
    {
        // Configure this based on your actual trash prefabs
        // These should match the trashName values in your Trash.cs components
        
        Dictionary<string, bool> itemRecyclableStatus = new Dictionary<string, bool>()
        {
            // RECYCLABLE ITEMS (isRecyclable = true in Trash.cs)
            // Paper Products
            { "Newspaper", true },
            { "Magazine", true },
            { "Office Paper", true },
            { "Scrap Paper", true },
            { "Cardboard Box", true },
            { "Clean Paper Cup", true },
            
            // Plastic Items
            { "Plastic Bottle", true },
            { "Water Bottle", true },
            { "Soda Bottle", true },
            { "Plastic Container", true },
            { "Clean Plastic Bag", true },
            
            // Metal Items
            { "Aluminum Can", true },
            { "Soda Can", true },
            { "Beer Can", true },
            { "Metal Can", true },
            { "Food Can", true },
            { "Steel Can", true },
            
            // Glass Items
            { "Glass Bottle", true },
            { "Wine Bottle", true },
            { "Beer Bottle", true },
            { "Glass Jar", true },
            
            // Generic recyclables
            { "Bottle", true },
            { "Can", true },
            { "Paper", true },
            { "Cardboard", true },
            
            // NON-RECYCLABLE ITEMS (isRecyclable = false in Trash.cs)
            // Food and Organic Waste
            { "Food Waste", false },
            { "Food Scraps", false },
            { "Banana Peel", false },
            { "Apple Core", false },
            { "Leftover Food", false },
            
            // Contaminated Paper Products
            { "Used Tissue", false },
            { "Dirty Paper Cup", false },
            { "Greasy Paper", false },
            { "Food-stained Paper", false },
            
            // Mixed Materials (cannot be separated)
            { "Candy Wrapper", false },
            { "Chip Bag", false },
            { "Foil Wrapper", false },
            { "Metallized Plastic", false },
            
            // Sanitary and Medical Items
            { "Diaper", false },
            { "Used Bandage", false },
            { "Medical Waste", false },
            { "Syringe", false },
            { "Cotton Swab", false },
            
            // Small and Problematic Items
            { "Cigarette Butt", false },
            { "Gum", false },
            { "Straw", false },
            { "Plastic Utensils", false },
            { "Toothpick", false },
            
            // Electronics (need special e-waste recycling)
            { "Battery", false },
            { "Phone", false },
            { "Electronics", false },
            { "Circuit Board", false },
            { "Old Phone", false },
            
            // Hazardous Materials
            { "Paint Can", false },
            { "Chemical Container", false },
            { "Motor Oil", false },
            { "Pesticide Bottle", false },
            { "Cleaning Product", false },
            
            // Textiles and Fabrics
            { "Old Clothes", false },
            { "Fabric Scraps", false },
            { "Shoes", false },
            { "Carpet Pieces", false },
            
            // Other Non-recyclables
            { "Ceramic", false },
            { "Broken Glass", false },
            { "Mirror", false },
            { "Light Bulb", false },
            { "Styrofoam", false }
        };

        // Check exact match first
        if (itemRecyclableStatus.ContainsKey(itemName))
        {
            if (showDebugLogs)
                Debug.Log($"Found exact match: '{itemName}' is recyclable: {itemRecyclableStatus[itemName]}");
            return itemRecyclableStatus[itemName];
        }

        // Check partial matches (for items with (Clone) or variations)
        string cleanName = itemName.Replace("(Clone)", "").Trim();
        foreach (var kvp in itemRecyclableStatus)
        {
            if (cleanName.Contains(kvp.Key) || kvp.Key.Contains(cleanName))
            {
                if (showDebugLogs)
                    Debug.Log($"Found partial match: '{itemName}' matches '{kvp.Key}' - recyclable: {kvp.Value}");
                return kvp.Value;
            }
        }

        // Default fallback for unknown items
        if (showDebugLogs)
            Debug.Log($"Unknown item '{itemName}' - defaulting to NON-recyclable");
        return false; // Default to non-recyclable if unknown
    }

    void StartDepositing()
    {
        if (depositCoroutine != null) return;

        if (showDebugLogs)
            Debug.Log("Starting deposit process");

        depositCoroutine = StartCoroutine(DepositCoroutine());
    }

    void StopDepositing()
    {
        if (depositCoroutine != null)
        {
            StopCoroutine(depositCoroutine);
            depositCoroutine = null;
        }

        isDepositing = false;
        
        if (depositSlider != null)
            depositSlider.gameObject.SetActive(false);

        if (showDebugLogs)
            Debug.Log("Stopped depositing");
    }

    IEnumerator DepositCoroutine()
    {
        isDepositing = true;

        if (depositSlider != null)
        {
            depositSlider.gameObject.SetActive(true);
            depositSlider.maxValue = depositTime;
            depositSlider.value = 0f;
        }

        if (showDebugLogs)
            Debug.Log("Deposit coroutine started");

        // Progress the deposit
        while (depositSlider.value < depositSlider.maxValue && Input.GetKey(interactKey))
        {
            if (depositSlider != null)
                depositSlider.value += Time.deltaTime;
            yield return null;
        }

        // If deposit completed
        if (depositSlider != null && depositSlider.value >= depositSlider.maxValue)
        {
            if (showDebugLogs)
                Debug.Log("Deposit completed, performing deposit");
            PerformDeposit();
        }

        StopDepositing();
    }

    void PerformDeposit()
    {
        var backpack = BackpackManager.Instance;
        var cashManager = CashManager.Instance;
        
        if (backpack == null || cashManager == null) 
        {
            if (showDebugLogs)
                Debug.Log("BackpackManager or CashManager is null");
            return;
        }

        var allItems = backpack.GetAllItems();
        int totalCashEarned = 0;
        int totalItemsDeposited = 0;

        List<string> itemsToRemove = new List<string>();

        if (showDebugLogs)
            Debug.Log($"Processing {allItems.Count} item types for deposit");

        // Process each item type
        foreach (var item in allItems)
        {
            if (IsItemCompatible(item.Key) && item.Value > 0)
            {
                // Calculate cash
                int cashPerItem = GetCashValueForItem(item.Key);
                int totalCash = cashPerItem * item.Value;
                
                totalCashEarned += totalCash;
                totalItemsDeposited += item.Value;
                
                itemsToRemove.Add(item.Key);

                if (showDebugLogs)
                    Debug.Log($"Will deposit {item.Value} x {item.Key} for ${totalCash}");
            }
        }

        // Remove items from backpack
        foreach (string itemName in itemsToRemove)
        {
            int count = backpack.GetItemCount(itemName);
            backpack.RemoveItems(itemName, count);
        }

        // Give cash reward
        if (totalCashEarned > 0)
        {
            cashManager.AddCash(totalCashEarned);
            
            string binType = acceptsRecyclable ? "Recyclable" : "Non-Recyclable";
            backpack.ShowNotification($"Deposited {totalItemsDeposited} items in {binType} bin! +${totalCashEarned}");
            
            if (showDebugLogs)
                Debug.Log($"Successfully deposited {totalItemsDeposited} items for ${totalCashEarned}");
        }
        else
        {
            backpack.ShowNotification("No compatible items to deposit!");
            if (showDebugLogs)
                Debug.Log("No items were deposited");
        }
    }

    int GetCashValueForItem(string itemName)
    {
        // Default cash values - you might want to make this more sophisticated
        Dictionary<string, int> cashValues = new Dictionary<string, int>()
        {
            { "bottle", 5 },
            { "can", 3 },
            { "paper", 2 },
            { "cardboard", 4 },
            { "food", 1 },
            { "diaper", 1 },
            { "cigarette", 1 },
            { "gum", 1 }
        };

        string itemLower = itemName.ToLower();
        
        foreach (var kvp in cashValues)
        {
            if (itemLower.Contains(kvp.Key))
                return kvp.Value;
        }

        return 2; // Default value
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (showDebugLogs)
                Debug.Log("Player entered trash can area");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopDepositing();
            
            if (interactPrompt != null)
                interactPrompt.gameObject.SetActive(false);
                
            if (showDebugLogs)
                Debug.Log("Player left trash can area");
        }
    }
}