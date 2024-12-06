using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private bool isBeingHeld = false;
    private Transform playerCamera;
    private float holdDistance = 2f;

    void Start()
    {
        playerCamera = Camera.main.transform; // Получаем ссылку на камеру
    }

    void Update()
    {
        if (isBeingHeld)
        {
            // Перемещаем объект к позиции перед камерой
            Vector3 targetPosition = playerCamera.position + playerCamera.forward * holdDistance;
            transform.position = targetPosition;
        }
    }

    public void Interact()
    {
        isBeingHeld = !isBeingHeld; // Переключаем состояние удержания объекта
    }
}