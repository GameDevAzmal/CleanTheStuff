using UnityEngine;
using System;

public class Trash : MonoBehaviour
{
    public float cleanUpTime = 5f;
    public Action onTrashDestroyed; 

    void OnDestroy()
    {
        if (onTrashDestroyed != null)
        {
            onTrashDestroyed.Invoke();
        }
    }
}
