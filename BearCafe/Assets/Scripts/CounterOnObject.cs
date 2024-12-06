using System;
using UnityEngine;  
using TMPro; // Необходимо для работы с TextMeshPro

public class CounterOnObject : MonoBehaviour
{
    public TextMeshPro textMeshPro; // Ссылка на компонент TextMeshPro
    public HoneySuckerController Counter;
    private int score = 0; // Начальное значение счётчика

    void Start()
    {
        Counter = HoneySuckerController.FindObjectOfType<HoneySuckerController>();
        UpdateScoreText(); // Обновляем текст в начале игры
    }

    void OnMouseDown() // Метод вызывается при нажатии на объект
    {
        IncreaseScore(); // Увеличиваем счётчик
    }

    void IncreaseScore()
    {
        score += Counter.HoneyCount; // Увеличиваем счётчик на 1
        UpdateScoreText(); // Обновляем текст на объекте
    }
    
    void UpdateScoreText()
    {
        textMeshPro.text = "Мёда: " + score; // Обновляем текстовый элемент
        Counter.HoneyCount = 0;
    }
}