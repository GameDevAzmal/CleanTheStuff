using UnityEngine;
using TMPro;

// Points script

public class COINS : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI coinsText;

    private bool isPanelOpen = false;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Toggle panel
            isPanelOpen = !isPanelOpen;
            panel.SetActive(isPanelOpen);
        }

        if (isPanelOpen && coinsText != null)
        {
            coinsText.text = "Trash Collected: " + TrashCleaner.countTrash;
        }
    }
}
