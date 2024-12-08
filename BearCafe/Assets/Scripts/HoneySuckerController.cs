using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class HoneySuckerController : MonoBehaviour
{
    public Camera mainCamera;
    public float fixedYPosition = 0.5f;
    public int HoneyCount = 0;
    public TextMeshProUGUI honeyCountText;
    public CounterOnObject Reset;
    public string username = "user_3024";
    public string gameUuid = "9e85841d-ac10-417c-898f-4910ad24ccca";
    public TextMeshProUGUI statusText;

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
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                Plane plane = new Plane(Vector3.up, new Vector3(0, fixedYPosition, 0));

                float enter;
                if (plane.Raycast(ray, out enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    transform.position = new Vector3(hitPoint.x, fixedYPosition, hitPoint.z);
                }
            }
        }
        UpdateHoneyCountText();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Honey"))
        {
            Destroy(other.gameObject);
            HoneyCount++;
            UpdateHoneyCountText();
            StartCoroutine(UpdatePlayerResources(HoneyCount));
        }
    }

    private IEnumerator UpdatePlayerResources(int honeyAmount)
    {
        if (string.IsNullOrEmpty(gameUuid) || string.IsNullOrEmpty(username))
        {
            yield break;
        }

        string url = $"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/{username}/";
        PlayerData updatedPlayerData = new PlayerData
        {
            resources = new Resources { honey = honeyAmount }
        };

        string jsonData = JsonUtility.ToJson(updatedPlayerData);
        UnityWebRequest putRequest = UnityWebRequest.Put(url, jsonData);
        putRequest.method = UnityWebRequest.kHttpVerbPUT;

        putRequest.SetRequestHeader("Content-Type", "application/json");

        yield return putRequest.SendWebRequest();

        if (putRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка при обновлении ресурсов игрока: " + putRequest.error);
        }
        else
        {
            PlayerData player = JsonUtility.FromJson<PlayerData>(putRequest.downloadHandler.text);
            Debug.Log($"Игрок {player.name} обновил ресурсы. Количество меда: {player.resources.honey}");
        }
    }

    private IEnumerator GetPlayerResources()
    {
        if (string.IsNullOrEmpty(gameUuid) || string.IsNullOrEmpty(username))
        {
            yield break;
        }

        string url = $"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/{username}/";
        UnityWebRequest getRequest = UnityWebRequest.Get(url);

        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка при получении ресурсов игрока: " + getRequest.error);
        }
        else
        {
            PlayerData player = JsonUtility.FromJson<PlayerData>(getRequest.downloadHandler.text);

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
            yield break;
        }

        string url = $"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/";
        UnityWebRequest getRequest = UnityWebRequest.Get(url);

        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка при получении списка игроков: " + getRequest.error);
        }
        else
        {
            PlayerData[] players = JsonHelper.FromJson<PlayerData>(getRequest.downloadHandler.text);

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
            honeyCountText.text = "Собрано меда: " + HoneyCount.ToString();
        }
        else
        {
            Debug.LogError("honeyCountText не установлен!");
        }
    }

    private string GenerateUniquePlayerName()
    {
        return "user_" + Random.Range(1000, 9999);
    }

    private IEnumerator CreatePlayer()
    {
        if (string.IsNullOrEmpty(gameUuid))
        {
            yield break;
        }

        string uniquePlayerName = GenerateUniquePlayerName();
        Debug.Log("Генерированное имя игрока: " + uniquePlayerName);

        string url = $"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/";

        PlayerData playerData = new PlayerData
        {
            name = uniquePlayerName,
            resources = new Resources
            {
                honey = HoneyCount
            }
        };

        string json = JsonUtility.ToJson(playerData);
        Debug.Log("Тело запроса: " + json);

        UnityWebRequest postRequest = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        postRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        postRequest.downloadHandler = new DownloadHandlerBuffer();
        postRequest.SetRequestHeader("Content-Type", "application/json");

        yield return postRequest.SendWebRequest();

        if (postRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка при создании игрока: " + postRequest.error);
            Debug.LogError("Ответ от сервера: " + postRequest.downloadHandler.text);
            if (statusText != null)
            {
                statusText.text = "Ошибка при создании игрока";
            }
        }
        else
        {
            Debug.Log("Игрок успешно создан: " + postRequest.downloadHandler.text);
            if (statusText != null)
            {
                statusText.text = "Игрок успешно создан!";
            }
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public Resources resources;
    }

    [System.Serializable]
    public class Resources
    {
        public int honey;
    }

    private IEnumerator DeleteAllPlayers()
    {
        UnityWebRequest getRequest = UnityWebRequest.Get($"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/");
        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка при получении списка игроков: " + getRequest.error);
            yield break;
        }

        Player[] players = JsonHelper.FromJson<Player>(getRequest.downloadHandler.text);

        if (players.Length == 0)
        {
            Debug.Log("Нет игроков для удаления.");
            yield break;
        }

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

        if (honeyCountText != null)
        {
            honeyCountText.text = "Все игроки удалены!";
        }
    }

    [System.Serializable]
    public class Player
    {
        public string name;
    }

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
