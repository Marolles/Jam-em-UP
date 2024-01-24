using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FameController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxFameValue = 100f;
    [SerializeField] private float defaultFameValue = 50f;

    public float fameGainedOnTickle = 10f;
    public float fameLostOnKill = 5f;

    private static float currentFameValue;

    public UnityEvent<float, float> onFameChange = new UnityEvent<float, float>();

    public static FameController instance;

    private void Awake()
    {
        //Singleton
        if (instance != null) { Destroy(instance); }
        instance = this;

        currentFameValue = defaultFameValue;
        UpdateFameBar();
    }

    public static float GetFameValue()
    {
        return currentFameValue;
    }

    public static float GetFameValueNormalized()
    {
        return currentFameValue / instance.maxFameValue;
    }

    public static float GetAngrinessNormalized()
    {
        return (1f - (GetFameValueNormalized() * 2));
    }

    public static float GetHappinessNormalized()
    {
        return (2 * (GetFameValueNormalized() - 0.5f));
    }

    public static void IncreaseFameValue(float _amount)
    {
        currentFameValue = Mathf.Clamp(currentFameValue + _amount, 0, instance.maxFameValue);
        instance.onFameChange.Invoke((_amount / instance.maxFameValue), GetFameValueNormalized());
        UpdateFameBar();
        CrowdManager.instance.UpdateCrowdColor();
    }

    public static void DecreaseFameValue(float _amount)
    {
        currentFameValue = Mathf.Clamp(currentFameValue - _amount, 0, instance.maxFameValue);
        instance.onFameChange.Invoke((-_amount / instance.maxFameValue), GetFameValueNormalized());
        UpdateFameBar();
        CrowdManager.instance.UpdateCrowdColor();
    }
    public static void ResetFameValue()
    {
        currentFameValue = 0;
        instance.onFameChange.Invoke(0, 0);
        UpdateFameBar();
        CrowdManager.instance.UpdateCrowdColor();
    }

    public static void UpdateFameBar()
    {
        FameBar.SetNormalizedValue(currentFameValue / instance.maxFameValue);
    }
}
