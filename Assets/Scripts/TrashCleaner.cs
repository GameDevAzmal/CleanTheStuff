using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrashCleaner : MonoBehaviour
{
    
    // The slider parameters

    public Slider cleanUpSlider; 
    public KeyCode cleanUpKey = KeyCode.E;
    private Trash currentTrash;

    private Coroutine cleaningCoroutine;

    void Start()
    {
        if (cleanUpSlider != null)
        {
            cleanUpSlider.gameObject.SetActive(false);
        }
    }


    // We basically clean the Trash Object (which is too destroy it, don't know how the Coroutine stuff happens)  
    void Update()
    {
        if (currentTrash != null && cleanUpSlider != null)
        {
            // Start cleaning when button is held
            if (Input.GetKey(cleanUpKey))
            {
                if (cleaningCoroutine == null) // start once
                {
                    cleaningCoroutine = StartCoroutine(CleanUpCoroutine());
                }
            }
            else
            {
                // Stop cleaning when button released
                if (cleaningCoroutine != null)
                {
                    StopCoroutine(cleaningCoroutine);
                    cleaningCoroutine = null;
                }
            }
        }
    }


    // This is the method that activates once the clean keybind is pressed

    IEnumerator CleanUpCoroutine()
    {
        cleanUpSlider.gameObject.SetActive(true);
        cleanUpSlider.maxValue = currentTrash.cleanUpTime;
        cleanUpSlider.value = currentTrash.cleanUpTime;

        while (cleanUpSlider.value > 0 && Input.GetKey(cleanUpKey))
        {
            cleanUpSlider.value -= Time.deltaTime;
            yield return null;
        }

        if (cleanUpSlider.value <= 0)
        {
            Destroy(currentTrash.gameObject);
            currentTrash = null;
            cleanUpSlider.gameObject.SetActive(false);
        }

        cleaningCoroutine = null;
    }

    // Trigger check for the Trash Object

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            currentTrash = other.GetComponent<Trash>();
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
