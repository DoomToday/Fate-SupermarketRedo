using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    private const int stoppedTimeScale = 0;
    private const int normalTimeScale = 1;

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
            TogglePause();
        }
    }
    public void TogglePause()
    {
        if (GameIsPaused)
        {
            pauseMenuUI.SetActive(false);
            removedUI.SetActive(true);
            Time.timeScale = normalTimeScale;
            GameIsPaused = false;
            gameUnpaused?.Invoke();
        }
        else
        {
            pauseMenuUI.SetActive(true);
            removedUI.SetActive(false);
            Time.timeScale = stoppedTimeScale;
            GameIsPaused = true;
        }
    }

    public void LoadMenu()
    {
        Debug.Log("Loading menu...");
        Time.timeScale = normalTimeScale;
        SceneManager.LoadScene(Constants.SCENE_MAIN_MENU);
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
