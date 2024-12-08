using UnityEngine;
using UnityEngine.SceneManagement;  // Для работы с сценами
using UnityEngine.UI;  // Для работы с UI элементами

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();
    }
}