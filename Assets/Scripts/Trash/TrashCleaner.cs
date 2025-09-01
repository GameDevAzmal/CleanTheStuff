using UnityEngine;
using UnityEngine.UI;
using TMPro; // ✅ Needed for TextMeshProUGUI
using System.Collections;

public class TrashCleaner : MonoBehaviour
{
    public Slider cleanUpSlider; 
    public KeyCode cleanUpKey = KeyCode.E;
    private Trash currentTrash;

    public TextMeshProUGUI trashCountText; 
    private Coroutine cleaningCoroutine;
    public static int countTrash; // total score

    void Start()
    {
        // Hide cleanup UI initially
        if (cleanUpSlider != null)
        {
            cleanUpSlider.gameObject.SetActive(false);
        }

        if (trashCountText != null)
        {
            trashCountText.text = "Trash: " + countTrash;
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

        // Always update UI
        if (trashCountText != null)
        {
            trashCountText.text = "Trash: " + countTrash;
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
            // ✅ Add score based on trash "weight"
            countTrash += currentTrash.points; 

            Destroy(currentTrash.gameObject);
            currentTrash = null;
            cleanUpSlider.gameObject.SetActive(false);
        }

        cleaningCoroutine = null;
    }

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
