using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TrashCleaner : MonoBehaviour
{
    public Slider cleanUpSlider; 
    public KeyCode cleanUpKey = KeyCode.E;
    private Trash currentTrash;
    public TextMeshProUGUI trashCount;
    private Coroutine cleaningCoroutine;
    public static int countTrash;

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

        trashCount.text = "Trash : " + countTrash;
    }

    IEnumerator CleanUpCoroutine()
    {
        
        cleanUpSlider.gameObject.SetActive(true);
        cleanUpSlider.maxValue = currentTrash.cleanUpTime;

        // Start at 0 (instead of full)
        cleanUpSlider.value = 0f;

        // Increase slider while key is held
        while (cleanUpSlider.value < cleanUpSlider.maxValue && Input.GetKey(cleanUpKey))
        {
            cleanUpSlider.value += Time.deltaTime;
            yield return null;
        }

        // Complete cleanup if slider reached max
        if (cleanUpSlider.value >= cleanUpSlider.maxValue)
        {
            Destroy(currentTrash.gameObject);
            countTrash++;
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
