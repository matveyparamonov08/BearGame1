using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoneyCollector : MonoBehaviour
{
    public GameObject honeyPrefab;
    public int honeyCount = 5;

    void Start()
    {
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
        Transform honeyTransform = honey.transform;
        honeyTransform.localScale *= 1.1f;

        if (honeyTransform.localScale.x < 0.1f)
        {
            Destroy(honey);
        }
    }
}