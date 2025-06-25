using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }
}
