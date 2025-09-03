using UnityEngine;
using TMPro;
using System.Text;

public class COINS : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI cashText;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.T;

    private bool isPanelOpen = false;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            // Toggle panel
            isPanelOpen = !isPanelOpen;
            panel.SetActive(isPanelOpen);
        }

        if (isPanelOpen)
        {
            UpdateInventoryDisplay();
            UpdateCashDisplay();
        }
    }

    void UpdateInventoryDisplay()
    {
        if (inventoryText == null || BackpackManager.Instance == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== BACKPACK ===");

        // Show slot-by-slot breakdown
        var slots = BackpackManager.Instance.GetInventorySlots();
        for (int i = 0; i < slots.Length; i++)
        {
            sb.AppendLine($"Slot {i + 1}: {slots[i].GetDisplayText()}");
        }

        sb.AppendLine();
        sb.AppendLine("=== TOTALS ===");

        // Show total counts per item type
        var allItems = BackpackManager.Instance.GetAllItems();
        if (allItems.Count > 0)
        {
            foreach (var kvp in allItems)
            {
                sb.AppendLine($"{kvp.Key}: x{kvp.Value}");
            }
        }
        else
        {
            sb.AppendLine("Backpack is empty");
        }

        inventoryText.text = sb.ToString();
    }

    void UpdateCashDisplay()
    {
        if (cashText == null || CashManager.Instance == null) return;

        int currentCash = CashManager.Instance.GetCash();
        cashText.text = $"Cash: ${currentCash}";
    }
}