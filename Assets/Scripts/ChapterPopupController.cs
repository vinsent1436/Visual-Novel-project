using UnityEngine;
using TMPro;
using System.Collections;
using System.Data;
using Mono.Data.Sqlite;

public class ChapterPopupController : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel; // Панель с всплывающим окном
    [SerializeField] private CanvasGroup layersCanvasGroup; // CanvasGroup для управления прозрачностью LAYERS
    [SerializeField] private TextMeshProUGUI chapterTitleText; // Текст для отображения заголовка главы
    [SerializeField] private TextMeshProUGUI chapterDescriptionText; // Текст для отображения описания главы
    [SerializeField] private float typingSpeed = 0.05f; // Скорость печати текста (секунды между буквами)
    [SerializeField] private CanvasGroup popupCanvasGroup; // CanvasGroup для управления прозрачностью popupPanel
    [SerializeField] private CanvasGroup backgroundDimmerCanvasGroup; // CanvasGroup для затемнения фона
    [SerializeField] private float fadeDuration = 1.5f; // Длительность анимации появления/исчезновения

    private string dbPath; // Путь к базе данных
    private Coroutine typingCoroutine; // Ссылка на текущую корутину печати текста
    
    private void Start()
    {

        // Устанавливаем путь к базе данных
        dbPath = "URI=file:" + Application.dataPath + "/DataBase/VN.db";

        // Загружаем первую главу из базы данных
        LoadChapterFromDatabase(1);

        // Настраиваем начальные параметры всплывающего окна и фона
        popupCanvasGroup.alpha = 1; // Полная видимость popupPanel
        backgroundDimmerCanvasGroup.alpha = 1; // Полное затемнение фона
        popupPanel.SetActive(true); // Активируем всплывающее окно

        // Убеждаемся, что слой LAYERS изначально невидим
        layersCanvasGroup.alpha = 0;
    }

    private void LoadChapterFromDatabase(int chapterID)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // SQL-запрос для получения данных главы по ID
                command.CommandText = "SELECT Name, Description FROM Episode WHERE ID = @id";
                command.Parameters.AddWithValue("@id", chapterID);

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Получаем название и описание главы
                        string title = reader.GetString(0); // Поле Name
                        string description = reader.GetString(1); // Поле Description

                        // Останавливаем предыдущую корутину печати текста, если она активна
                        if (typingCoroutine != null)
                        {
                            StopCoroutine(typingCoroutine);
                        }

                        // Запускаем печать текста заголовка и описания
                        typingCoroutine = StartCoroutine(TypeText(chapterTitleText, title));
                        StartCoroutine(TypeText(chapterDescriptionText, description));
                    }
                    else
                    {
                        Debug.LogWarning("Глава с указанным ID не найдена.");
                    }
                }
            }
            connection.Close();
        }
    }

    private IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText)
    {
        // Настройки текстового компонента
        textComponent.enableWordWrapping = true; // Включить перенос слов
        textComponent.overflowMode = TextOverflowModes.Overflow; // Разрешить переполнение текста
        textComponent.text = ""; // Очищаем текст

        // Печатаем текст по буквам
        foreach (char letter in fullText)
        {
            textComponent.text += letter; // Добавляем букву к тексту
            yield return new WaitForSeconds(typingSpeed); // Ожидаем указанное время

            // Если пользователь щелкает мышью, сразу показываем весь текст
            if (Input.GetMouseButtonDown(0))
            {
                textComponent.text = fullText; // Отображаем весь текст
                yield break; // Прерываем корутину
            }
        }
    }

    public void OnStartButtonClicked()
    {
        // Запускаем анимацию закрытия всплывающего окна и появления LAYERS
        StartCoroutine(HandlePopupAndLayers());
    }

    private IEnumerator HandlePopupAndLayers()
    {
        // Анимация исчезновения всплывающего окна и затемнения фона
        yield return StartCoroutine(FadeOutBackgroundDimmer());
         // Анимация появления слоя LAYERS
        FadeInLayers();
        yield return StartCoroutine(FadeOutPanel());
        
    }

    private IEnumerator FadeOutPanel()
    {
        // Постепенно уменьшаем прозрачность popupPanel
        while (popupCanvasGroup.alpha > 0)
        {
            popupCanvasGroup.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        popupPanel.SetActive(false); // Деактивируем панель после исчезновения
        Debug.Log("Корутина закончена.");
    }

    private IEnumerator FadeOutBackgroundDimmer()
    {
        // Постепенно уменьшаем прозрачность фона
        while (backgroundDimmerCanvasGroup.alpha > 0)
        {
            backgroundDimmerCanvasGroup.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        backgroundDimmerCanvasGroup.gameObject.SetActive(false); // Деактивируем фон после исчезновения
    }

    private void FadeInLayers()
    {
        // Постепенно увеличиваем прозрачность слоя LAYERS
        layersCanvasGroup.gameObject.SetActive(true);
        while (layersCanvasGroup.alpha < 1)
        {
            layersCanvasGroup.alpha += Time.deltaTime / fadeDuration;
        }
        layersCanvasGroup.alpha = 1; // Устанавливаем полную прозрачность
    }
}
