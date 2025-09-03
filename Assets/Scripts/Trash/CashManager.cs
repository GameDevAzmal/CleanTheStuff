using UnityEngine;
using TMPro;
using System;

public class CashManager : MonoBehaviour
{
    public static CashManager Instance;

    [Header("Cash Settings")]
    public int startingCash = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI cashText;

    private int currentCash;

    // Events for cash changes
    public static Action<int> OnCashChanged;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize cash
        currentCash = startingCash;
        UpdateCashUI();
    }

    void Start()
    {
        // Load saved cash (if you have save system later)
        LoadCash();
        UpdateCashUI();
    }

    // Add cash (from deposits)
    public void AddCash(int amount)
    {
        if (amount <= 0) return;

        currentCash += amount;
        UpdateCashUI();
        OnCashChanged?.Invoke(currentCash);

        Debug.Log($"Earned ${amount}! Total: ${currentCash}");
    }

    // Spend cash (for shop later)
    public bool SpendCash(int amount)
    {
        if (amount <= 0 || currentCash < amount)
            return false;

        currentCash -= amount;
        UpdateCashUI();
        OnCashChanged?.Invoke(currentCash);

        Debug.Log($"Spent ${amount}! Remaining: ${currentCash}");
        return true;
    }

    // Get current cash amount
    public int GetCash()
    {
        return currentCash;
    }

    // Check if player can afford something
    public bool CanAfford(int amount)
    {
        return currentCash >= amount;
    }

    // Update the cash UI
    void UpdateCashUI()
    {
        if (cashText != null)
        {
            cashText.text = $"Cash: ${currentCash}";
        }
    }

    // Save cash (basic - you can enhance this later)
    public void SaveCash()
    {
        PlayerPrefs.SetInt("PlayerCash", currentCash);
        PlayerPrefs.Save();
    }

    // Load cash (basic - you can enhance this later)
    public void LoadCash()
    {
        currentCash = PlayerPrefs.GetInt("PlayerCash", startingCash);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveCash();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SaveCash();
    }
}