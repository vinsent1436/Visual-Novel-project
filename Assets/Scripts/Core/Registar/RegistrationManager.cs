using UnityEngine;
using TMPro; // Подключение пространства имен TextMeshPro
using System.Data;
using Mono.Data.Sqlite;

public class RegistrationManager : MonoBehaviour
{
    public TMP_InputField emailInput; // Поле ввода email
    public TMP_InputField passwordInput; // Поле ввода пароля
    public TMP_InputField confirmPasswordInput; // Поле для подтверждения пароля
    public TextMeshProUGUI feedbackText; // Текст для вывода сообщений пользователю

    private string dbPath; // Путь к базе данных

    private void Start()
    {
        // Устанавливаем путь к базе данных
        dbPath = Application.dataPath + "/DataBase/VN.db";
    }

    public void Register()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        // Проверка валидности email
        if (!IsValidEmail(email))
        {
            feedbackText.text = "Введите корректный Email.";
            return;
        }

        // Проверка совпадения паролей
        if (password != confirmPassword)
        {
            feedbackText.text = "Пароли не совпадают.";
            return;
        }

        // Проверка длины пароля
        if (password.Length < 6)
        {
            feedbackText.text = "Пароль должен быть не менее 6 символов.";
            return;
        }

        // Попытка регистрации пользователя
        bool success = RegisterUser(email, password);
        if (success)
        {
            feedbackText.text = "Вы успешно зарегистрировались!";
        }
        else
        {
            feedbackText.text = "Ошибка: данный Email уже используется.";
        }
    }

    private bool RegisterUser(string email, string password)
    {
        try
        {
            // Подключение к базе данных
            using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
            {
                connection.Open();

                // Проверка, существует ли пользователь с таким Email
                string checkUserQuery = "SELECT COUNT(*) FROM User WHERE Email = @Email";
                using (var command = new SqliteCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    long count = (long)command.ExecuteScalar();
                    if (count > 0)
                    {
                        return false; // Пользователь уже существует
                    }
                }

                // Добавление нового пользователя
                string insertUserQuery = "INSERT INTO User (Email, Password) VALUES (@Email, @Password)";
                using (var command = new SqliteCommand(insertUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.ExecuteNonQuery();
                }
            }
            return true; // Регистрация успешна
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка подключения к базе данных: {ex.Message}");
            feedbackText.text = "Ошибка подключения к базе данных.";
            return false;
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
