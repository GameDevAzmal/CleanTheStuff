using UnityEngine;
using System;

public class Trash : MonoBehaviour
{
    public float cleanUpTime = 5f;
    public Action onTrashDestroyed; // Event for when trash is destroyed

    void OnDestroy()
    {
        // Notify listeners when trash is destroyed
        if (onTrashDestroyed != null)
        {
            onTrashDestroyed.Invoke();
        }
    }
}
