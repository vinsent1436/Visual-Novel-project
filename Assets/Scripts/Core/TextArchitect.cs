using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextArchitect
{
    // Компонент TextMeshPro для UI
    private TextMeshProUGUI tmpro_ui;
    // Компонент TextMeshPro для отображения текста в мировом пространстве
    private TextMeshPro tmpro_world;
    
    // Это свойство проверяет, присвоен ли tmpro_ui; если это так, используется tmpro_ui, иначе — tmpro_world
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;
    
    // Это свойство возвращает текущий текст, отображаемый в tmpro
    public string currentText => tmpro.text;
    
    // Текст, который будет отображаться в конце
    public string targetText { get; private set; } = "";
    
    // Существующий текст, который может быть до добавления нового текста
    public string preText { get; private set; } = "";
    
    private int preTextLength = 0;

    // Полный текст (preText + targetText), который будет построен
    public string fullTargetText => preText + targetText;

    // Перечисление для различных методов построения текста
    public enum BuildMethod
    {
        instant, // Мгновенное отображение текста
        typewriter, // Эффект печатной машинки (по одному символу)
        fade // Эффект затухания текста
    }

    // Метод отображения текста (по умолчанию типографический эффект)
    public BuildMethod buildMethod = BuildMethod.typewriter;

    // Цвет текста
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    // Скорость печатной машинки, контролируемая переменной speedMultiplier
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }

    private const float baseSpeed = 1; // Базовая скорость печатной машинки
    private float speedMultiplier = 1; // Множитель для скорости печатной машинки

    // Определяет, сколько символов нужно обрабатывать за один цикл в зависимости от скорости
    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    
    private int characterMultiplier = 1; // Множитель для символов

    // Флаг для ускорения или пропуска отображения текста
    public bool hurryUp = false;

    // Конструктор для инициализации с компонентом TextMeshProUGUI
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    // Конструктор для инициализации с компонентом TextMeshPro
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    // Этот метод устанавливает targetText для отображения и запускает процесс построения текста
    public Coroutine Build(string text)
    {
        preText = ""; // Сбросить предшествующий текст
        targetText = text;

        Stop(); // Остановить предыдущий процесс отображения текста

        // Запускает корутину для построения текста
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    // Этот метод добавляет новый текст к текущему
    public Coroutine Append(string text)
    {
        preText = tmpro.text; // Сохранить текущий текст как preText
        targetText = text;

        Stop(); // Остановить предыдущий процесс отображения текста

        // Запускает корутину для построения текста
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    // Переменная для хранения корутины, управляющей процессом построения текста
    private Coroutine buildProcess = null;

    // Свойство, которое возвращает булево значение, указывающее, выполняется ли процесс построения текста
    public bool isBuilding => buildProcess != null;

    // Этот метод останавливает текущий процесс отображения текста
    public void Stop()
    {
        // Если нет активной корутины, выходим
        if (!isBuilding)
        {
            return;
        }
        
        // Останавливаем корутину
        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    // Корутина для построения текста в зависимости от выбранного метода
    IEnumerator Building()
    {
        Prepear(); // Подготовить текст перед отображением

        // Выбираем метод отображения текста
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }

        OnComplete(); // Завершаем процесс отображения
    }

    // Этот метод выполняется, когда процесс завершен
    private void OnComplete()
    {
        buildProcess = null;
        hurryUp = false; // Отключаем флаг для ускорения
    }

    // Этот метод принудительно завершает процесс отображения текста
    public void ForseComplit()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.fade:
                tmpro.ForceMeshUpdate();
                break;
        }

        Stop(); // Останавливаем текущий процесс
        OnComplete(); // Завершаем процесс
    }

    // Метод для подготовки текста перед его отображением в зависимости от метода
    private void Prepear()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;
        }
    }

    // Мгновенное отображение текста
    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color; // Обновить цвет
        tmpro.text = fullTargetText; // Установить полный текст
        tmpro.ForceMeshUpdate(); // Применить изменения
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount; // Показать все символы
    }

    // Подготовка для эффекта печатной машинки
    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color; // Обновить цвет
        tmpro.maxVisibleCharacters = 0; // Начинаем с нулевого символа
        tmpro.text = preText;

        // Если preText не пустой, то применяем ForceMeshUpdate
        if (preText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText; // Добавляем новый текст
        tmpro.ForceMeshUpdate(); // Применить изменения
    }

    // Подготовка для эффекта затухания
    private void Prepare_Fade()
    {
        tmpro.text = preText; // Устанавливаем предшествующий текст
        preTextLength = preText != "" ? tmpro.textInfo.characterCount : 0;

        tmpro.text = targetText; // Устанавливаем новый текст
        tmpro.maxVisibleCharacters = int.MaxValue; // Все символы изначально невидимы
        tmpro.ForceMeshUpdate(); // Применить изменения

        TMP_TextInfo textInfo = tmpro.textInfo;
        Color colorVisible = new Color(textColor.r, textColor.g, textColor.b, 1);
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);

        // Инициализация массива цветов для символов
        Color32[] vertexColor = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                continue;
            }

            if (i < preTextLength)
            {
                // Для символов из preText делаем их видимыми
                for (int v = 0; v < 4; v++)
                {
                    vertexColor[charInfo.vertexIndex + v] = colorVisible;
                }
            }
            else
            {
                // Для остальных символов устанавливаем невидимость
                for (int v = 0; v < 4; v++)
                {
                    vertexColor[charInfo.vertexIndex + v] = colorHidden;
                }
            }
        }

        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32); // Обновляем данные вершин
    }

    // Корутина для эффекта печатной машинки
    private IEnumerator Build_Typewriter()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            // Печать символов по одному
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 4 : charactersPerCycle;
            yield return new WaitForSeconds(0.015f / speed); // Задержка между символами
        }
    }

    // Корутина для эффекта затухания текста
    private IEnumerator Build_Fade()
    {
        // Получаем информацию о тексте
        TMP_TextInfo textInfo = tmpro.textInfo;
        Color32[] vertexColor = textInfo.meshInfo[0].colors32;

        float fadeSpeed = 2f;

        while (true)
        {
            // Плавно увеличиваем прозрачность каждого символа
            bool finished = true;
            for (int i = preTextLength; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                Color32 color = vertexColor[charInfo.vertexIndex];

                if (color.a < 255)
                {
                    finished = false;
                    color.a = (byte)Mathf.Clamp(color.a + fadeSpeed, 0, 255); // Увеличиваем альфа-канал
                    for (int j = 0; j < 4; j++)
                    {
                        vertexColor[charInfo.vertexIndex + j] = color;
                    }
                }
            }

            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32); // Обновляем данные

            if (finished)
            {
                break;
            }

            yield return new WaitForEndOfFrame(); // Ждем следующего кадра
        }
    }
}