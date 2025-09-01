using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody rb;
    private Vector3 movement;

    public Slider cleanUpSlider; // Assign in Inspector
    public KeyCode cleanUpKey = KeyCode.E;
    private GameObject currentTrash;

    public float cleanUpSpeed = 50f; // How fast the slider decreases
    public float initialSliderValue = 100f; // Customize initial value

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (cleanUpSlider != null)
        {
            cleanUpSlider.gameObject.SetActive(false); // Hide slider at start
            cleanUpSlider.maxValue = initialSliderValue;
            cleanUpSlider.value = initialSliderValue;
        }
    }

    void Update()
    {
        HandleMovementInput();
        HandleCleanUpInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleMovementInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");
    }

    void MovePlayer()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void HandleCleanUpInput()
    {
        if (Input.GetKeyDown(cleanUpKey) && currentTrash != null && cleanUpSlider != null)
        {
            StartCoroutine(CleanUpCoroutine());
        }
    }

    IEnumerator CleanUpCoroutine()
    {
        // Show slider and reset value
        cleanUpSlider.gameObject.SetActive(true);
        cleanUpSlider.value = initialSliderValue;

        // Gradually decrease the slider
        while (cleanUpSlider.value > 0)
        {
            cleanUpSlider.value -= Time.deltaTime * cleanUpSpeed;
            yield return null; // wait for next frame
        }

        // Destroy trash when slider reaches 0
        Destroy(currentTrash);
        currentTrash = null;

        // Hide slider after cleanup
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
