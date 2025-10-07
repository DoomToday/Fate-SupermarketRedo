using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject removedUI;
    [SerializeField] private GameObject gameFinishedUI;
    public static event Action gameUnpaused;

    private void Start()
    {
        SupermarketManager.gameFinished += FinishGame;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        removedUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        gameUnpaused?.Invoke();
    }
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        removedUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Debug.Log("Loading menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
    public void FinishGame()
    {
        gameFinishedUI.SetActive(true);
        removedUI.SetActive(false);
    }
}
