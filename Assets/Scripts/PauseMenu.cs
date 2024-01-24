using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static PauseMenu instance;
    [SerializeField] private Transform panel;

    private bool opened = false;

    private void Awake()
    {
        if (instance != null) { Destroy(instance); }
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!opened)
            {
                PauseGame();
            } else
            {
                ResumeGame();
            }
        }
    }

    public static void PauseGame()
    {
        instance.panel.gameObject.SetActive(true);
        instance.opened = true;
        Time.timeScale = 0f;
    }

    public static void ResumeGame()
    {
        instance.panel.gameObject.SetActive(false);
        instance.opened = false;
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ResetGame()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
