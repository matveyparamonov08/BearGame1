using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIManager : MonoBehaviour
{
    private string baseUrl = "https://2025.nti-gamedev.ru/api/games/";
    private string gameUUID = "11111111-1111-1111-1111-111111111111"; // Ваш UUID игры

    public IEnumerator CreatePlayer(string playerName, string resourceKey = null, int resourceValue = 0)
    {
        string apiUrl = baseUrl + gameUUID + "/players/";
        
        // Формируем тело запроса
        string jsonData;
        if (resourceKey != null)
        {
            jsonData = "{\"name\": \"" + playerName + "\", \"resources\": {\"" + resourceKey + "\": " + resourceValue + "}}";
        }
        else
        {
            jsonData = "{\"name\": \"" + playerName + "\"}";
        }

        // Логируем тело запроса
        Debug.Log("Отправляем запрос: " + apiUrl);
        Debug.Log("Тело запроса: " + jsonData);

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
                Debug.Log("Ответ POST: Игрок создан - " + webRequest.downloadHandler.text);
            }
        }
    }

    void Start()
    {
        // Создание игрока с ресурсами
        StartCoroutine(CreatePlayer("user_with_apples", "apples", 1));

        // Создание игрока без ресурсов
        // StartCoroutine(CreatePlayer("user1"));
    }
}
