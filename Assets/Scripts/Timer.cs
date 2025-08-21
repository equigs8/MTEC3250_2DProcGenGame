using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float currentTime;
    private static bool isRunning;

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        isRunning = true;
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            timerText.text = string.Format("{00:00}:{01:00}", time.Minutes, time.Seconds);
        }      
    }

    public static void PauseTimer(bool status)
    {
        isRunning = !status;
    }
}
