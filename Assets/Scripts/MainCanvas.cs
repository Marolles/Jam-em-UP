using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas instance;

    private static Canvas canvas;

    private void Awake()
    {
        if (instance == null) { Debug.Log("Setting canvas instance"); instance = this; } else { Debug.Log("Destroying canvas"); Destroy(this); }
        canvas = GetComponent<Canvas>();
    }

    public static Canvas GetCanvas()
    {
        return canvas;
    }
}
