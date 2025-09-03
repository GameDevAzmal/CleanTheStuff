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

        // TEMPORARY: Accept ALL items for testing
        // Remove this after confirming the deposit system works
        if (showDebugLogs)
            Debug.Log($"TEMP: Accepting all items. Item '{itemName}' is compatible with bin (acceptsRecyclable: {acceptsRecyclable})");
        return true;

        /*
        // ORIGINAL CODE - Uncomment after testing
        
        // Recyclable items (case insensitive)
        string[] recyclableItems = { "bottle", "can", "paper", "cardboard", "plastic" };
        // Non-recyclable items
        string[] nonRecyclableItems = { "food", "diaper", "cigarette", "gum", "organic" };

        string itemLower = itemName.ToLower();
        
        bool itemIsRecyclable = System.Array.Exists(recyclableItems, x => itemLower.Contains(x));
        bool itemIsNonRecyclable = System.Array.Exists(nonRecyclableItems, x => itemLower.Contains(x));

        if (showDebugLogs)
        {
            Debug.Log($"Item '{itemName}' -> Recyclable: {itemIsRecyclable}, Non-Recyclable: {itemIsNonRecyclable}");
        }

        // If this bin accepts recyclable and item is recyclable
        if (acceptsRecyclable && itemIsRecyclable) return true;
        // If this bin accepts non-recyclable and item is non-recyclable  
        if (!acceptsRecyclable && itemIsNonRecyclable) return true;

        return false;
        */
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