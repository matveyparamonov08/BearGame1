using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class HoneySuckerController : MonoBehaviour
{
    public Camera mainCamera; // Ссылка на основную камеру
    public float fixedYPosition = 0.5f; // Фиксированная позиция по оси Y
    public int HoneyCount = 0; // Счётчик меда
    public TextMeshProUGUI honeyCountText; // Ссылка на текстовый элемент для отображения счётчика
    public CounterOnObject Reset;
    public string username = "user_3024";
    
    // Параметры для создания игрока
    public string gameUuid = "9e85841d-ac10-417c-898f-4910ad24ccca";

    public TextMeshProUGUI statusText; // Для отображения статуса запроса

    private void Start()
    {
        Reset = FindObjectOfType<CounterOnObject>();
        UpdateHoneyCountText();
        StartCoroutine(GetPlayerList());
        StartCoroutine(GetPlayerResources());
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Debug.Log("Курсор разблокирован и видим.");

            // Проверяем, нажата ли левая кнопка мыши
            if (Input.GetMouseButton(0)) // 0 - это левая кнопка мыши
            {
                // Получаем позицию курсора на экране
                Vector3 mousePosition = Input.mousePosition;

                // Преобразуем экранные координаты в мировые координаты
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                Plane plane =
                    new Plane(Vector3.up, new Vector3(0, fixedYPosition, 0)); // Создаем плоскость на фиксированной высоте

                float enter;
                if (plane.Raycast(ray, out enter))
                {
                    // Получаем точку пересечения луча и плоскости
                    Vector3 hitPoint = ray.GetPoint(enter);
                    // Устанавливаем позицию медососа на точку пересечения, фиксируя Y
                    transform.position = new Vector3(hitPoint.x, fixedYPosition, hitPoint.z);
                }
            }
        }
        UpdateHoneyCountText();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Honey")) // Проверяем, является ли объект медом
        {
            Destroy(other.gameObject); // Уничтожаем объект меда
            HoneyCount++;
            UpdateHoneyCountText(); // Обновляем текст счётчика меда
            StartCoroutine(UpdatePlayerResources(HoneyCount));
        }
    }

    private IEnumerator UpdatePlayerResources(int honeyAmount)
    {
        if (string.IsNullOrEmpty(gameUuid) || string.IsNullOrEmpty(username))
        {
            Debug.LogError("Game UUID или имя игрока не заданы.");
            yield break;
        }

        // Формируем URL для обновления ресурсов игрока
        string url = $"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/{username}/";

        // Создаем объект с новыми ресурсами
        PlayerData updatedPlayerData = new PlayerData
        {
            resources = new Resources { honey = honeyAmount }
        };

        // Преобразуем объект в JSON
        string jsonData = JsonUtility.ToJson(updatedPlayerData);

        // Отправляем PUT-запрос с обновленными данными
        UnityWebRequest putRequest = UnityWebRequest.Put(url, jsonData);
        putRequest.method = UnityWebRequest.kHttpVerbPUT; // Указываем, что это PUT-запрос

        // Устанавливаем заголовок для указания типа контента
        putRequest.SetRequestHeader("Content-Type", "application/json");

        // Ждем ответа от сервера
        yield return putRequest.SendWebRequest();

        if (putRequest.result != UnityWebRequest.Result.Success)
        {
            // Если запрос не удался, выводим ошибку
            Debug.LogError("Ошибка при обновлении ресурсов игрока: " + putRequest.error);
        }
        else
        {
            // Если запрос успешен, выводим ответ от сервера
            Debug.Log("Ответ от сервера: " + putRequest.downloadHandler.text);

            // Преобразуем полученный ответ в объект PlayerData
            PlayerData player = JsonUtility.FromJson<PlayerData>(putRequest.downloadHandler.text);

            // Выводим обновленные ресурсы игрока
            Debug.Log($"Игрок {player.name} обновил ресурсы. Количество меда: {player.resources.honey}");
        }
    }
    private IEnumerator GetPlayerResources()
    {
        if (string.IsNullOrEmpty(gameUuid) || string.IsNullOrEmpty(username))
        {
            Debug.LogError("Game UUID или имя игрока не заданы.");
            yield break;
        }

        // Формируем URL для получения ресурсов игрока
        string url = $"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/{username}/";

        // Отправляем GET-запрос
        UnityWebRequest getRequest = UnityWebRequest.Get(url);

        // Ждем ответа от сервера
        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            // Если запрос не удался, выводим ошибку
            Debug.LogError("Ошибка при получении ресурсов игрока: " + getRequest.error);
        }
        else
        {
            // Если запрос успешен, выводим полученный ответ
            Debug.Log("Ответ от сервера: " + getRequest.downloadHandler.text);

            // Преобразуем полученный JSON в объект PlayerData
            PlayerData player = JsonUtility.FromJson<PlayerData>(getRequest.downloadHandler.text);

            // Выводим информацию о ресурсах игрока
            if (player.resources != null)
            {
                Debug.Log($"Игрок: {player.name}, Ресурсы: {player.resources.honey}");
            }
            else
            {
                Debug.Log($"Игрок: {player.name}, Нет ресурсов.");
            }
        }
    }
    private IEnumerator GetPlayerList()
    {
        if (string.IsNullOrEmpty(gameUuid))
        {
            Debug.LogError("Game UUID не задан.");
            yield break;
        }

        // URL для получения списка игроков
        string url = $"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/";

        // Отправляем GET-запрос на сервер
        UnityWebRequest getRequest = UnityWebRequest.Get(url);

        // Ждем ответа от сервера
        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            // Если запрос не удался, выводим ошибку
            Debug.LogError("Ошибка при получении списка игроков: " + getRequest.error);
        }
        else
        {
            // Если запрос успешен, выводим полученный ответ
            Debug.Log("Ответ от сервера: " + getRequest.downloadHandler.text);

            // Преобразуем полученный JSON в список игроков
            PlayerData[] players = JsonHelper.FromJson<PlayerData>(getRequest.downloadHandler.text);

            // Выводим информацию о каждом игроке
            foreach (var player in players)
            {
                Debug.Log($"Игрок: {player.name}, Мёд: {player.resources?.honey ?? 0}");
            }
        }
    }
    private void UpdateHoneyCountText()
    {
        if (honeyCountText != null)
        {
            honeyCountText.text = "Собрано меда: " + HoneyCount.ToString(); // Обновляем отображаемый текст
        }
        else
        {
            Debug.LogError("honeyCountText не установлен!"); // Сообщение об ошибке, если текст не установлен
        }
    }

    // Корутин для создания игрока
    private string GenerateUniquePlayerName()
    {
        // Генерация уникального имени с использованием случайного числа
        return "user_" + Random.Range(1000, 9999);
    }
    
    private IEnumerator CreatePlayer()
    {
        if (string.IsNullOrEmpty(gameUuid))
        {
            Debug.LogError("Game UUID не задан.");
            yield break;
        }

        // Генерация уникального имени для нового игрока
        string uniquePlayerName = GenerateUniquePlayerName();
        Debug.Log("Генерированное имя игрока: " + uniquePlayerName);

        string url = $"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/";

        // Создаем JSON-данные для запроса
        PlayerData playerData = new PlayerData
        {
            name = uniquePlayerName,
            resources = new Resources
            {
                honey = HoneyCount // Устанавливаем количество яблок
            }
        };

        // Логируем тело запроса перед отправкой
        string json = JsonUtility.ToJson(playerData);
        Debug.Log("Тело запроса: " + json);  // Логирование тела запроса

        // Отправляем POST-запрос
        UnityWebRequest postRequest = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        postRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        postRequest.downloadHandler = new DownloadHandlerBuffer();
        postRequest.SetRequestHeader("Content-Type", "application/json");

        // Ждем ответа от сервера
        yield return postRequest.SendWebRequest();

        if (postRequest.result != UnityWebRequest.Result.Success)
        {
            // Логируем подробную ошибку и ответ сервера
            Debug.LogError("Ошибка при создании игрока: " + postRequest.error);
            Debug.LogError("Ответ от сервера: " + postRequest.downloadHandler.text); // Логирование ответа от сервера
            if (statusText != null)
            {
                statusText.text = "Ошибка при создании игрока";
            }
        }
        else
        {
            // Логируем успешный ответ
            Debug.Log("Игрок успешно создан: " + postRequest.downloadHandler.text);
            if (statusText != null)
            {
                statusText.text = "Игрок успешно создан!";
            }
        }
    }



    // Класс для данных игрока
    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public Resources resources;
    }

    // Класс для ресурсов игрока
    [System.Serializable]
    public class Resources
    {
        public int honey;
    }

    // Корутин для удаления всех игроков
    private IEnumerator DeleteAllPlayers()
    {
        UnityWebRequest getRequest = UnityWebRequest.Get($"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/");
        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка при получении списка игроков: " + getRequest.error);
            yield break;
        }

        // Парсим ответ JSON, получаем массив игроков
        Player[] players = JsonHelper.FromJson<Player>(getRequest.downloadHandler.text);

        if (players.Length == 0)
        {
            Debug.Log("Нет игроков для удаления.");
            yield break;
        }

        // Удаляем каждого игрока
        foreach (var player in players)
        {
            string playerName = player.name;

            UnityWebRequest deleteRequest = UnityWebRequest.Delete($"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/{playerName}");
            yield return deleteRequest.SendWebRequest();

            if (deleteRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Ошибка при удалении игрока {playerName}: " + deleteRequest.error);
            }
            else
            {
                Debug.Log($"Игрок {playerName} успешно удалён.");
            }
        }

        // После удаления всех игроков обновляем статус
        if (honeyCountText != null)
        {
            honeyCountText.text = "Все игроки удалены!";
        }
    }

    // Класс для парсинга одного игрока
    [System.Serializable]
    public class Player
    {
        public string name;
    }

    // Вспомогательный класс для парсинга массива JSON
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{\"items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] items;
        }
    }
}
