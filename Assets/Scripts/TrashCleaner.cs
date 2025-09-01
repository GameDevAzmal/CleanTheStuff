using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrashCleaner : MonoBehaviour
{
    public Slider cleanUpSlider; 
    public KeyCode cleanUpKey = KeyCode.E;
    private Trash currentTrash;

    private Coroutine cleaningCoroutine;

    void Start()
    {
        // Hide cleanup UI initially
        if (cleanUpSlider != null)
        {
            cleanUpSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (currentTrash != null && cleanUpSlider != null)
        {
            if (Input.GetKey(cleanUpKey))
            {
                // Start cleaning process (only once)
                if (cleaningCoroutine == null)
                {
                    cleaningCoroutine = StartCoroutine(CleanUpCoroutine());
                }
            }
            else
            {
                // Stop cleaning when key released
                if (cleaningCoroutine != null)
                {
                    StopCoroutine(cleaningCoroutine);
                    cleaningCoroutine = null;
                }
            }
        }
    }

    IEnumerator CleanUpCoroutine()
    {
        // Setup cleanup UI
        cleanUpSlider.gameObject.SetActive(true);
        cleanUpSlider.maxValue = currentTrash.cleanUpTime;
        cleanUpSlider.value = currentTrash.cleanUpTime;

        // Countdown while key is held
        while (cleanUpSlider.value > 0 && Input.GetKey(cleanUpKey))
        {
            cleanUpSlider.value -= Time.deltaTime;
            yield return null;
        }

        // Complete cleanup if timer reached zero
        if (cleanUpSlider.value <= 0)
        {
            Destroy(currentTrash.gameObject);
            currentTrash = null;
            cleanUpSlider.gameObject.SetActive(false);
        }

        cleaningCoroutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        // Detect when player enters trash area
        if (other.CompareTag("Trash"))
        {
            currentTrash = other.GetComponent<Trash>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Clean up when player leaves trash area
        if (other.CompareTag("Trash") && currentTrash != null && other.gameObject == currentTrash.gameObject)
        {
            currentTrash = null;
            cleanUpSlider.gameObject.SetActive(false);

            // Stop any active cleaning process
            if (cleaningCoroutine != null)
            {
                StopCoroutine(cleaningCoroutine);
                cleaningCoroutine = null;
            }
        }
    }
}