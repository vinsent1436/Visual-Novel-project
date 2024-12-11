using UnityEngine;

public class LogoutManager : MonoBehaviour
{
    public GameObject registrationCanvas;

    public void Logout()
    {
        // Сброс текущей сессии.
        PlayerPrefs.DeleteKey("LoggedInEmail");
        PlayerPrefs.Save();

        // Показываем окно регистрации.
        registrationCanvas.SetActive(true);
    }
}
