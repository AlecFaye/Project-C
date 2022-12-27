using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimer : MonoBehaviour
{
    private TextMeshProUGUI timerText;

    private float timeValue = 0f;
    
    public bool IsTimerStarted = false;


    private void Awake() {
        timerText = GetComponent<TextMeshProUGUI>();
        DisplayTime();
    }

    private void Update() {
        if (IsTimerStarted == false) return;

        UpdateTime();
        DisplayTime();
    }

    private void UpdateTime() { timeValue += Time.deltaTime; }

    private void DisplayTime() {
        float seconds = Mathf.FloorToInt(timeValue % 60);
        float minutes = Mathf.FloorToInt((timeValue / 60) % 60);
        float hours = Mathf.FloorToInt(timeValue / 3600);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
