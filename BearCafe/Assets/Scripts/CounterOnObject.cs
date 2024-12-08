using System;
using UnityEngine;  
using TMPro;

public class CounterOnObject : MonoBehaviour
{
    public TextMeshPro textMeshPro;
    public HoneySuckerController Counter;
    private int score = 0;

    void Start()
    {
        Counter = HoneySuckerController.FindObjectOfType<HoneySuckerController>();
        UpdateScoreText();
    }

    void OnMouseDown()
    {
        IncreaseScore();
    }

    void IncreaseScore()
    {
        score += Counter.HoneyCount;
        UpdateScoreText();
    }
    
    void UpdateScoreText()
    {
        textMeshPro.text = "Мёд: " + score;
        Counter.HoneyCount = 0;
    }
}