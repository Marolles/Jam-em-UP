using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    private enum SpectatorStatus { NEUTRAL, HAPPY, ANGRY };
    [SerializeField] private Renderer[] renderers;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color happyColor;
    [SerializeField] private Color angryColor;

    [SerializeField] private float colorChangeDuration = 0.1f;
    [SerializeField] private Ease colorChangeEase = Ease.InBounce;

    private SpectatorStatus currentStatus = SpectatorStatus.NEUTRAL;

    private void Awake()
    {
        CrowdManager.spectators.Add(this);
        SetNeutral(true);
    }

    private void Update()
    {
        if (!ArenaController.IsFrozen())
            LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        if (PlayerController.instance != null)
        {
            Vector3 _lookedPosition = PlayerController.instance.transform.position;
            _lookedPosition.y = transform.position.y;
            transform.LookAt(_lookedPosition);
        }
    }

    public void SetNeutral(bool _forceChange = false)
    {
        if (!_forceChange && currentStatus == SpectatorStatus.NEUTRAL) return;
        currentStatus = SpectatorStatus.NEUTRAL;
        transform.DOKill(false);
        foreach (Renderer _r in renderers)
        {
            _r.material.DOColor(defaultColor, colorChangeDuration).SetEase(colorChangeEase);
        }
    }

    public void SetHappy()
    {
        if (currentStatus == SpectatorStatus.HAPPY) return;
        currentStatus = SpectatorStatus.HAPPY;
        transform.DOKill(false);
        foreach (Renderer _r in renderers)
        {
            _r.material.DOColor(happyColor, colorChangeDuration).SetEase(colorChangeEase);
        }
    }

    public void SetAngry()
    {
        if (currentStatus == SpectatorStatus.ANGRY) return;
        currentStatus = SpectatorStatus.ANGRY;
        transform.DOKill(false);
        foreach (Renderer _r in renderers)
        {
            _r.material.DOColor(angryColor, colorChangeDuration).SetEase(colorChangeEase);
        }
    }
}
