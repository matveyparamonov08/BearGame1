using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras; // Массив камер для переключения
    private int currentCameraIndex = 0;
    public MonoBehaviour[] scriptsToDisable; // Ссылка на скрипт, который нужно отключить
    public GameObject uiElement; // Ссылка на UI-элемент
    
    void Start()
    {
        SwitchCamera(currentCameraIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Замените 'E' на нужную вам клавишу
        {
            currentCameraIndex++;
            if (currentCameraIndex >= cameras.Length)
            {
                currentCameraIndex = 0; // Сброс индекса, если он превышает количество камер
            }
            SwitchCamera(currentCameraIndex);
        }
    }

    void SwitchCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].enabled = (i == index); // Включаем только текущую камеру
        }

        // Блокировка или разблокировка скрипта и курсора в зависимости от активной камеры
        if (index == 0) // Например, первая камера — это игровая камера
        {
            EnableScripts();
            LockCursor(); // Заблокировать курсор и скрыть его
            HideUIElement(); // Скрыть UI-элемент
        }
        else // Вторая камера — это камера интерфейса или меню
        {
            DisableScripts();
            UnlockCursor(); // Разблокировать курсор и сделать его видимым
            
            ShowUIElement(); // Показать UI-элемент
            
        }
    }

    void DisableScripts()
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false; // Отключаем указанные скрипты
            }
        }
    }

    void EnableScripts()
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = true; // Включаем указанные скрипты
            }
        }
        
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Блокируем курсор
        Cursor.visible = false; // Скрываем курсор
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Разблокируем курсор
        Cursor.visible = true; // Показываем курсор
    }
    void HideUIElement()
    {
        if (uiElement != null)
        {
            uiElement.SetActive(false); // Скрываем UI-элемент
        }
    }

    void ShowUIElement()
    {
        if (uiElement != null)
        {
            uiElement.SetActive(true); // Показываем UI-элемент
        }
    }
}   