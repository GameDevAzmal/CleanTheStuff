using UnityEngine;
using TMPro;
using System.Text; // for StringBuilder

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
            // Build the text
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Points: " + TrashCleaner.totalPoints);

            // Also show breakdown of collected trash
            foreach (var kvp in TrashCleaner.trashCounts)
            {
                sb.AppendLine(kvp.Key + " x" + kvp.Value);
            }

            coinsText.text = sb.ToString();
        }
    }
}
