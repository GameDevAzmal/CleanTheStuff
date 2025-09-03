using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrashCleaner : MonoBehaviour
{
    [Header("Collection Settings")]
    public Slider cleanUpSlider; 
    public KeyCode cleanUpKey = KeyCode.E;
    
    private Trash currentTrash;
    private Coroutine cleaningCoroutine;

    void Start()
    {
        if (cleanUpSlider != null)
            cleanUpSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (currentTrash != null && cleanUpSlider != null)
        {
            if (Input.GetKey(cleanUpKey))
            {
                if (cleaningCoroutine == null)
                    cleaningCoroutine = StartCoroutine(CleanUpCoroutine());
            }
            else
            {
                if (cleaningCoroutine != null)
                {
                    StopCoroutine(cleaningCoroutine);
                    cleaningCoroutine = null;
                    cleanUpSlider.gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator CleanUpCoroutine()
    {
        if (currentTrash == null) yield break;

        // Check if we can add this item to backpack before starting
        string itemName = currentTrash.GetCleanName();
        bool canAdd = !BackpackManager.Instance.IsBackpackFull();
        
        if (!canAdd)
        {
            BackpackManager.Instance.ShowNotification("Backpack Full!");
            cleaningCoroutine = null;
            yield break;
        }

        cleanUpSlider.gameObject.SetActive(true);
        cleanUpSlider.maxValue = currentTrash.cleanUpTime;
        cleanUpSlider.value = 0f;

        // Progress the cleaning
        while (cleanUpSlider.value < cleanUpSlider.maxValue && Input.GetKey(cleanUpKey))
        {
            // Double-check backpack space during cleaning
            if (BackpackManager.Instance.IsBackpackFull())
            {
                BackpackManager.Instance.ShowNotification("Backpack Full!");
                cleanUpSlider.gameObject.SetActive(false);
                cleaningCoroutine = null;
                yield break;
            }

            cleanUpSlider.value += Time.deltaTime;
            yield return null;
        }

        // If cleaning completed
        if (cleanUpSlider.value >= cleanUpSlider.maxValue)
        {
            // Try to add item to backpack
            bool success = BackpackManager.Instance.TryAddItem(
                itemName, 
                currentTrash.cashValue, 
                currentTrash.stackLimit
            );

            if (success)
            {
                // Destroy the trash object
                Destroy(currentTrash.gameObject);
                currentTrash = null;
            }
            else
            {
                BackpackManager.Instance.ShowNotification("Backpack Full!");
            }

            cleanUpSlider.gameObject.SetActive(false);
        }

        cleaningCoroutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            Trash trash = other.GetComponent<Trash>();
            if (trash != null)
            {
                currentTrash = trash;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trash") && currentTrash != null && other.gameObject == currentTrash.gameObject)
        {
            currentTrash = null;
            cleanUpSlider.gameObject.SetActive(false);

            if (cleaningCoroutine != null)
            {
                StopCoroutine(cleaningCoroutine);
                cleaningCoroutine = null;
            }
        }
    }
}