using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FameBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    [SerializeField] private Image fillLerpedBarPositive;
    [SerializeField] private Image fillLerpedBarNegative;
    [SerializeField] private float fillLerpDuration = 0.1f;
    [SerializeField] private Ease fillLerpPositiveEase = Ease.InSine;
    [SerializeField] private Ease fillLerpNegativeEase = Ease.OutSine;

    [SerializeField] private Color fillPositiveColor;
    [SerializeField] private Color fillNegativeColor;

    public static FameBar instance;
    private static float currentFillValue;

    Tweener positiveTweenLerp;
    Tweener negativeTweenLerp;

    private void Awake()
    {
        if (instance != null) { Destroy(instance); }
        instance = this;

        ResetBar();
    }

    public static void ResetBar()
    {
        instance.fillBar.fillAmount = 0.5f;
        instance.fillLerpedBarNegative.fillAmount = 0.5f;
        instance.fillLerpedBarPositive.fillAmount = 0.5f;
    }

    public static void SetNormalizedValue(float _value)
    {
        if (instance.negativeTweenLerp != null) instance.negativeTweenLerp.Kill();
        if (instance.positiveTweenLerp != null) instance.positiveTweenLerp.Kill();

        instance.fillBar.fillAmount = _value;
        instance.fillLerpedBarPositive.color = _value < 0.5f ? instance.fillNegativeColor : instance.fillPositiveColor;
        if (_value > currentFillValue)
        {            
            //Increase in fame
            instance.fillLerpedBarNegative.fillAmount = _value;
            instance.positiveTweenLerp = instance.fillLerpedBarPositive.DOFillAmount(_value, instance.fillLerpDuration).SetEase(instance.fillLerpPositiveEase);
        }
        else if (_value < currentFillValue)
        {
            //Decrease in fame
            instance.fillLerpedBarPositive.fillAmount = _value;
            instance.negativeTweenLerp = instance.fillLerpedBarNegative.DOFillAmount(_value, instance.fillLerpDuration).SetEase(instance.fillLerpNegativeEase);
        }
        currentFillValue = _value;
    }
}
