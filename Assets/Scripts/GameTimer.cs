using UnityEngine;
using UnityEngine.UI;   // Only if you want to show the timer on screen

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public bool useRandomTime = true;   // Toggle random vs fixed
    public float minTime = 30f;         // Min random time
    public float maxTime = 60f;         // Max random time
    public float fixedTime = 45f;       // Fixed time if random disabled

    private float timer;

    [Header("UI (Optional)")]
    public Text timerText; // Assign in inspector if you want to show countdown

    void Start()
    {
        if (useRandomTime)
        {
            timer = Random.Range(minTime, maxTime);
        }
        else
        {
            timer = fixedTime;
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString();

        if (timer <= 0f)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in Editor
#else
            Application.Quit(); // Quit build
#endif
        }
    }
}
