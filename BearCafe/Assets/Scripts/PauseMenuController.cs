using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Панель паузы
    private bool isPaused = false; // Флаг паузы
    public MonoBehaviour cameraControlScript; // Скрипт управления камерой
    public MonoBehaviour HoneySucker;
    public MonoBehaviour CameraSwitcher;

    void Start()
    {
        pauseMenuUI.SetActive(false); // Скрываем меню паузы при старте
    }

    void Update()
    {
        // Проверяем нажатие клавиши Escape для переключения состояния паузы
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // Если игра на паузе, возобновляем
            }
            else
            {
                PauseGame(); // Если игра не на паузе, ставим на паузу
                
            }
        }
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Показываем меню паузы
        Time.timeScale = 0f; // Останавливаем время игры
        isPaused = true; // Обновляем флаг

        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = false; // Отключаем управление камерой
            HoneySucker.enabled = false;
            CameraSwitcher.enabled = false;
        }
        Cursor.lockState = CursorLockMode.None; // Разблокируем курсор
        Cursor.visible = true; // Делаем курсор видимым

        // Блокируем и скрываем курсор
        
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Скрываем меню паузы
        Time.timeScale = 1f; // Возвращаем нормальное время
        isPaused = false; // Обновляем флаг

        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = true; // Включаем управление камерой\
            HoneySucker.enabled = true;
            CameraSwitcher.enabled = true;
        }

        // Скрываем курсор и блокируем его
        Cursor.lockState = CursorLockMode.Locked; // Блокируем курсор
        Cursor.visible = false; // Прячем курсор
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit(); // Закрытие игры
    }
}