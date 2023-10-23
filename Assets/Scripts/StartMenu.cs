using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public string gameScene;
    public void StartGame()
    {
        Debug.Log("Loading game...");
        SceneManager.LoadScene(gameScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
