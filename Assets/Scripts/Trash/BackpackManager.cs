using UnityEngine;
using TMPro;
using System.Collections;

public class BackpackManager : MonoBehaviour
{
    public static BackpackManager Instance;

    public int backpackCapacity = 10;
    public TextMeshProUGUI notificationText;
    public float notificationDuration = 2f;

    private Coroutine notificationCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public bool IsBackpackFull()
    {
        int total = 0;
        foreach (var kvp in TrashCleaner.trashCounts)
            total += kvp.Value;

        return total >= backpackCapacity;
    }

    public void ShowNotification(string message)
    {
        if (notificationText == null) return;

        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(NotificationRoutine(message));
    }

    private IEnumerator NotificationRoutine(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(notificationDuration);

        notificationText.gameObject.SetActive(false);
    }
}
