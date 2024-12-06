using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoneyCollector : MonoBehaviour
{
    public GameObject honeyPrefab; // Префаб меда
    public int honeyCount = 5; // Количество меда для появления

    void Start()
    {
        // Убираем мед в начале
        RemoveAllHoney();
    }

    public void OnCollectHoneyButtonPressed()
    {
        SpawnHoney();
    }

    void RemoveAllHoney()
    {
        foreach (GameObject honey in GameObject.FindGameObjectsWithTag("Honey"))
        {
            Destroy(honey);
        }
    }

    void SpawnHoney()
    {
        for (int i = 0; i < honeyCount; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
            Instantiate(honeyPrefab, randomPosition, Quaternion.identity).tag = "Honey";
        }
    }
    
    public void CollectHoney(GameObject honey)
    {
        // Получаем компонент Transform объекта меда
        Transform honeyTransform = honey.transform;

        // Уменьшаем размер меда
        honeyTransform.localScale *= 1.1f;

        // Проверяем, достиг ли размер минимального значения
        if (honeyTransform.localScale.x < 0.1f) // Минимальный размер (можно настроить)
        {
            Destroy(honey); // Уничтожаем объект меда
        }
    }

}