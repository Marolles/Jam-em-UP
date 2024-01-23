using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform panel;

    public static GameOverPanel instance;

    private bool inputDetection;
    private bool active;

    private void Awake()
    {
        if (instance != null) { Destroy(instance); }
        instance = this;

        active = false;
        inputDetection = false;
    }
    public void ShowGameOverPanel()
    {
        if (!active)
        {
            panel.gameObject.SetActive(true);
            animator.SetTrigger("Show");
            active = true;
            Invoke("ActivateInputDetection", 0.5f);
        }
    }

    public void ActivateInputDetection()
    {
        inputDetection = true;
    }

    private void Update()
    {
        if (inputDetection && Input.anyKey)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
