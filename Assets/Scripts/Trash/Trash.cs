using UnityEngine;
using System;

public class Trash : MonoBehaviour
{
    [Header("Trash Properties")]
    public string trashName = "Bottle"; // Name of the trash item
    public bool isRecyclable = true; // true = recyclable, false = non-recyclable
    public int cashValue = 5; // Cash earned when deposited correctly
    public int stackLimit = 10; // Maximum items per inventory slot
    
    [Header("Collection Settings")]
    public float cleanUpTime = 2f; // Time needed to collect this trash
    
    public Action onTrashDestroyed; // Event for when trash is destroyed

    void OnDestroy()
    {
        // Notify listeners when trash is destroyed
        if (onTrashDestroyed != null)
        {
            onTrashDestroyed.Invoke();
        }
    }

    // Get trash type for display
    public string GetTrashType()
    {
        return isRecyclable ? "Recyclable" : "Non-Recyclable";
    }

    // Get clean name without (Clone)
    public string GetCleanName()
    {
        return trashName.Replace("(Clone)", "").Trim();
    }
}