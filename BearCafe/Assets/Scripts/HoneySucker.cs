using UnityEngine;

public class HoneySucker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Honey"))
        {
            HoneyCollector collector = FindObjectOfType<HoneyCollector>();
            collector.CollectHoney(other.gameObject);
            
        }
    }
}