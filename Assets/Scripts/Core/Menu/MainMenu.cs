using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [ContextMenu("Play Game")]
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    [ContextMenu("Exit Game")]
    public void ExitGame()
    {
        Debug.Log("Игра закрылась");
        Application.Quit();
    }
}

