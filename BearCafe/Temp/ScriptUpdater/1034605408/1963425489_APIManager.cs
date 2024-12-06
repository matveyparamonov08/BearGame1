using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIManager : MonoBehaviour
{
    private string baseUrl = "https://2025.nti-gamedev.ru/api/games/";
    private string gameUUID = "11111111-1111-1111-1111-111111111111";  // Здесь ваш UUID игры

    // Метод для создания игрока (POST)
    public IEnumerator CreatePlayer(string playerName, string resourceKey = null, int resourceValue = 0)
    {
        string apiUrl = baseUrl + gameUUID + "/players/";

        string jsonData;
        if (resourceKey != null)
        {
            jsonData = "{\"name\": \"" + playerName + "\", \"resources\": {\"" + resourceKey + "\": " + resourceValue + "}}";
        }
        else
        {
            jsonData = "{\"name\": \"" + playerName + "\"}";
        }

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

    // Метод для получения игроков (GET)
    public IEnumerator GetPlayers()
    {
        string apiUrl = baseUrl + gameUUID + "/players/";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Ошибка GET: " + webRequest.error);
                Debug.LogError("Ответ сервера: " + webRequest.downloadHandler.text);
            }
            else
            {
                Debug.Log("Ответ GET: " + webRequest.downloadHandler.text);
            }
        }
    }

    // Метод для обновления игрока (PUT)
    public IEnumerator UpdatePlayer(string playerUUID, string newResourceKey, int newResourceValue)
    {
        string apiUrl = baseUrl + gameUUID + "/players/" + playerUUID;

        string jsonData = "{\"resources\": {\"" + newResourceKey + "\": " + newResourceValue + "}}";

        using (UnityWebRequest webRequest = UnityWebRequest.Put(apiUrl, jsonData))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Ошибка PUT: " + webRequest.error);
                Debug.LogError("Ответ сервера: " + webRequest.downloadHandler.text);
            }
            else
            {
                Debug.Log("Ответ PUT: Игрок обновлён - " + webRequest.downloadHandler.text);
            }
        }
    }

    // Метод для удаления игрока (DELETE)
    public IEnumerator DeletePlayer(string playerUUID)
    {
        string apiUrl = baseUrl + gameUUID + "/players/" + playerUUID;

        using (UnityWebRequest webRequest = UnityWebRequest.Delete(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Ошибка DELETE: " + webRequest.error);
                Debug.LogError("Ответ сервера: " + webRequest.downloadHandler.text);
            }
            else
            {
                Debug.Log("Ответ DELETE: Игрок удалён - " + webRequest.downloadHandler.text);
            }
        }
    }

    // Вызываем методы в Start для тестирования
    void Start()
    {
        // Пример: создание игрока с ресурсами
        StartCoroutine(CreatePlayer("user_with_apples", "apples", 1));

        // Пример: получение списка игроков
        StartCoroutine(GetPlayers());

        // Пример: обновление данных игрока
        string playerUUID = "uuid_игрока";  // Укажите реальный UUID игрока
        StartCoroutine(UpdatePlayer(playerUUID, "apples", 10));

        // Пример: удаление игрока
        StartCoroutine(DeletePlayer(playerUUID));
    }
}
