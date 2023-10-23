using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;

    public GameObject PauseMenuUI;

    public string menuScene;

    public Tilemap fog;
    //private TilemapRenderer fog;
    private bool fogOn;

    void OnAwake()
    {
        //fog = fogMap.GetComponent<TilemapRenderer>();
        //fog.forceRenderingOff = true;
        fogOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }

        if (fogOn)
        {
            fog.ClearAllTiles();
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }

    void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void ToggleFog()
    {
        if (fogOn)
        {
            //fog.SetColor(255, 255, 255, 0);
            fogOn = false;
        } else
        {
//            fog.SetColor(255, 255, 255, 255);
            fogOn = true;
        }
    }

    public void LoadMenu()
    {
        Debug.Log("Loading menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
