using UnityEngine;
using System;

public class Trash : MonoBehaviour
{

        
    // The clean time for the Trash
    public float cleanUpTime = 2f;
    public Action onTrashDestroyed; // Event for when trash is destroyed

    // Destroy method for destroying the instance, it also Invokes it back to make the Trash object spawn again
    void OnDestroy()
    {
        // Notify listeners when trash is destroyed
        if (onTrashDestroyed != null)
        {
            onTrashDestroyed.Invoke();
        }
    }
}
