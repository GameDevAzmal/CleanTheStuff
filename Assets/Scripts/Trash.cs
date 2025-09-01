using UnityEngine;
using System;

public class Trash : MonoBehaviour
{

        
    // The clean time for the Trash
    public float cleanUpTime = 5f;
    public Action onTrashDestroyed; 

    // Destroy method for destroying the instance, it also Invokes it back to make the Trash object spawn again
    void OnDestroy()
    {
        if (onTrashDestroyed != null)
        {
            onTrashDestroyed.Invoke();
        }
    }
}
