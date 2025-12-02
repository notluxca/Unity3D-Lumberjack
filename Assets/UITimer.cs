using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float startTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time + 300f; // 5 minutes
    }

    // Update is called once per frame
    void Update()
    {
        float timeRemaining = startTime - Time.time;
        int minutes = (int)timeRemaining / 60;
        int seconds = (int)timeRemaining % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}

