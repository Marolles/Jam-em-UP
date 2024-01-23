using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasIntro : MonoBehaviour
{
    int introStep;
    public GameObject[] disappearStep2;
    public GameObject[] appearStep2;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (introStep == 0)
            {
                for (int i = 0; i < disappearStep2.Length; i++)
                {
                    disappearStep2[i].SetActive(false);
                }
                for (int i = 0; i < appearStep2.Length; i++)
                {
                    appearStep2[i].SetActive(true);
                }
                introStep++;
            }
            else
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
}
