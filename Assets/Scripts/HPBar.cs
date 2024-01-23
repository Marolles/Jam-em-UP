using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPBar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fillLerpDuration = 1f;
    [SerializeField] private Ease fillEase;

    [Header("References")]
    [SerializeField] private Image filling;
    [SerializeField] private Image lerpedFilling;


    private Tween currentTween;

    public void UpdateBar(float _HPPercent)
    {
        filling.fillAmount = _HPPercent;
        if (currentTween != null) currentTween.Kill(true);
        currentTween = lerpedFilling.DOFillAmount(_HPPercent, fillLerpDuration).SetEase(fillEase);
    }

    public void HideHPBar(float _delay)
    {
        Invoke("HideBar", _delay);
    }

    private void HideBar()
    {
        transform.gameObject.SetActive(false);
    }
}
