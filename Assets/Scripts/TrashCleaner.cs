using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrashCleaner : MonoBehaviour
{
    public Slider cleanUpSlider;
    public KeyCode cleanUpKey = KeyCode.E;
    private GameObject currentTrash;

    public float cleanUpSpeed = 50f;
    public float initialSliderValue = 100f;

    void Start()
    {
        if (cleanUpSlider != null)
        {
            cleanUpSlider.gameObject.SetActive(false);
            cleanUpSlider.maxValue = initialSliderValue;
            cleanUpSlider.value = initialSliderValue;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(cleanUpKey) && currentTrash != null && cleanUpSlider != null)
        {
            StartCoroutine(CleanUpCoroutine());
        }
    }

    IEnumerator CleanUpCoroutine()
    {
        cleanUpSlider.gameObject.SetActive(true);
        cleanUpSlider.value = initialSliderValue;

        while (cleanUpSlider.value > 0)
        {
            cleanUpSlider.value -= Time.deltaTime * cleanUpSpeed;
            yield return null;
        }

        Destroy(currentTrash);
        currentTrash = null;
        cleanUpSlider.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            currentTrash = other.gameObject;
        }
    }
}
