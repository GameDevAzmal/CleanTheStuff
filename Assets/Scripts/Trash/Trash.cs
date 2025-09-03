using UnityEngine;
using System;

public class Trash : MonoBehaviour
{
    [Header("Trash Properties")]
    public string trashName = "Bottle";      // Display name
    public bool isRecyclable = true;         // true = recyclable, false = non-recyclable
    public int cashValue = 5;                // Cash earned per item
    public int stackLimit = 10;              // Max per inventory slot

    [Header("Collection Settings")]
    public float cleanUpTime = 2f;           // Time needed to collect/deposit

    public Action onTrashDestroyed;          // Event triggered when trash is destroyed

    void OnDestroy()
    {
        onTrashDestroyed?.Invoke();
    }

    public string GetTrashType()
    {
        return isRecyclable ? "Recyclable" : "Non-Recyclable";
    }

    public string GetCleanName()
    {
        return trashName.Replace("(Clone)", "").Trim();
    }
}
