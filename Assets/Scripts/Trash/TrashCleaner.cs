using UnityEngine;
using UnityEngine.UI;
using TMPro; // ✅ Needed for TextMeshProUGUI
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TrashCleaner : MonoBehaviour
{
    public Slider cleanUpSlider; 
    public KeyCode cleanUpKey = KeyCode.E;
    private Trash currentTrash;

    public TextMeshProUGUI trashCountText; 
    private Coroutine cleaningCoroutine;
    public Dictionary<string, int> trashCounts = new Dictionary<string, int>();

    public static int backpackCapacity = 1;
    public static int totalPoints; // total score
    // private TrashManager trashManager;

    void Start()
    {
        // Hide cleanup UI initially
        if (cleanUpSlider != null)
        {
            cleanUpSlider.gameObject.SetActive(false);
        }

        if (trashCountText != null)
        {
            trashCountText.text = $"Trash: {GetTotalTrashCollected()}";
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
            trashCountText.text = $"Trash: {GetTotalTrashCollected()}";
        }
    }

    IEnumerator CleanUpCoroutine()
    {
        if (currentTrash == null) yield break;
        // Setup cleanup UI
        cleanUpSlider.gameObject.SetActive(true);
        cleanUpSlider.maxValue = currentTrash.cleanUpTime;
        cleanUpSlider.value = currentTrash.cleanUpTime;

        // Countdown while key is held
        while (cleanUpSlider.value > 0 && Input.GetKey(cleanUpKey))
        {

            if (GetTotalTrashCollected() >= backpackCapacity)
            {
                Debug.Log("Backpack full!");
                cleanUpSlider.gameObject.SetActive(false);
                yield break;
            }
            else
            {
                cleanUpSlider.value -= Time.deltaTime;
                yield return null;
            }
        }

        // This is basically keeps track of the trash in the backpack and 
        if (cleanUpSlider.value <= 0)
        {
            if (GetTotalTrashCollected() >= backpackCapacity)
            {
                Debug.Log("Backpack full!");
            }

            else
            {
                // string trashType = currentTrash.name;
                string trashType = currentTrash.name.Replace("(Clone)", "");

                if (trashCounts.ContainsKey(trashType))
                {
                    trashCounts[trashType]++;
                }
                else
                {
                    trashCounts[trashType] = 1;
                }

                totalPoints += currentTrash.points;
                Debug.Log($"Collected {trashType}, total = {trashCounts[trashType]}");
                Debug.Log($"Total trash: {totalPoints}");
                Destroy(currentTrash.gameObject);
            }

            

            currentTrash = null;
            cleanUpSlider.gameObject.SetActive(false);
        }

        cleaningCoroutine = null;
    }

    // Gets total no of trash in the trashCounts dictionary
    public int GetTotalTrashCollected()
    {
        return trashCounts.Values.Sum();
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
