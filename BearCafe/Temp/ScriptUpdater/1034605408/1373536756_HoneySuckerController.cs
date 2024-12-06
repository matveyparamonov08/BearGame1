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
    private string gameUuid = "9e85841d-ac10-417c-898f-4910ad24ccca"; // Замените на ваш UUID игры
    private string apiUrl;

    private void Start()
    {
        Reset = FindObjectOfType<CounterOnObject>();
        UpdateHoneyCountText();
        apiUrl = $"https://2025.nti-gamedev.ru/api/games/{gameUuid}/players/";
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Debug.Log("Курсор разблокирован и видим.");
        
            if (Input.GetMouseButton(0)) // 0 - это левая кнопка мыши
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                Plane plane = new Plane(Vector3.up, new Vector3(0, fixedYPosition, 0));

                if (plane.Raycast(ray, out float enter))
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
        if (other.CompareTag("Honey")) // Если объект - мед
        {
            Destroy(other.gameObject);
            HoneyCount++;
            UpdateHoneyCountText();
            SendPlayerDataToServer(); // Отправляем данные игрока на сервер
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

    // Метод для отправки данных игрока на сервер
    private void SendPlayerDataToServer()
    {
        StartCoroutine(PostPlayerData());
    }

    private IEnumerator PostPlayerData()
    {
        // Формируем тело запроса
        var playerData = new
        {
            name = "user_with_apples", // Используйте подходящее имя
            resources = new
            {
                apples = HoneyCount // Отправляем количество собранного меда как количество яблок
            }
        };

        string jsonData = JsonUtility.ToJson(playerData);

        // Создаем запрос
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(apiUrl, jsonData))
        {
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json");

            // Отправляем запрос
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Запрос успешно отправлен: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Ошибка отправки запроса: " + www.error);
            }
        }
    }
}
