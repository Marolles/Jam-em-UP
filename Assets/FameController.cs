using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FameController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxFameValue = 100f;

    public float fameGainedOnTickle = 10f;
    public float fameLostOnKill = 5f;

    private static float currentFameValue = 0;

    public static FameController instance;

    private void Awake()
    {
        //Singleton
        if (instance != null) { Destroy(instance); }
        instance = this;

        currentFameValue = 0;
    }

    public static void IncreaseFameValue(float _amount)
    {
        currentFameValue = Mathf.Clamp(currentFameValue + _amount, 0, instance.maxFameValue);
        UpdateFameBar();
    }

    public static void DecreaseFameValue(float _amount)
    {
        currentFameValue = Mathf.Clamp(currentFameValue - _amount, 0, instance.maxFameValue);
        UpdateFameBar();
    }
    public static void ResetFameValue()
    {
        currentFameValue = 0;
        UpdateFameBar();
    }

    public static void UpdateFameBar()
    {
        FameBar.SetNormalizedValue(currentFameValue / instance.maxFameValue);
    }
}
