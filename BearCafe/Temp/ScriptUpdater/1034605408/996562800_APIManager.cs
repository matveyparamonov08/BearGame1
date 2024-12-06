using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIManager : MonoBehaviour
{
    private string baseUrl = "https://2025.nti-gamedev.ru/api/games/";
    private string gameUUID = "your-game-uuid"; // Замените на ваш UUID

    // GET-запрос для получения списка игроков
    public IEnumerator GetPlayers()
    {
        string apiUrl = baseUrl + gameUUID + "/players/";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Ошибка GET: " + webRequest.error);
            }
            else
            {
                Debug.Log("Ответ GET: " + webRequest.downloadHandler.text);
            }
        }
    }

    // POST-запрос для добавления нового игрока
    public IEnumerator AddPlayer(string playerName, int score)
    {
        string apiUrl = baseUrl + gameUUID + "/players/";
        string jsonData = "{\"name\": \"" + playerName + "\", \"score\": " + score + "}";

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(apiUrl, jsonData))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Ошибка POST: " + webRequest.error);
                Debug.LogError("Ответ сервера: " + webRequest.downloadHandler.text);
            }
            else
            {
                Debug.Log("Ответ POST: Игрок добавлен - " + webRequest.downloadHandler.text);
            }
        }
    }

    // Вызываем запросы из Start для демонстрации
    void Start()
    {
        StartCoroutine(GetPlayers()); // Вызов GET-запроса

        // Пример POST-запроса
        StartCoroutine(AddPlayer("Player1", 100)); 
    }
}
