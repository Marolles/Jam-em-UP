using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    public enum SpectatorStatus { NEUTRAL, HAPPY, ANGRY };
    [SerializeField] private Renderer[] renderers;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color defaultColor2;

    [SerializeField] private Color happyColor;
    [SerializeField] private Color happyColor2;

    [SerializeField] private Color angryColor;
    [SerializeField] private Color angryColor2;

    [SerializeField] private float colorChangeDuration = 0.1f;
    [SerializeField] private Ease colorChangeEase = Ease.InBounce;

    private SpectatorStatus currentStatus = SpectatorStatus.NEUTRAL;

    [SerializeField] private List<GameObject> happyFaces;
    [SerializeField] private List<GameObject> angryFaces;

    [SerializeField] private Animator animator;

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

    public SpectatorStatus GetStatus()
    {
        return currentStatus;
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

        Color _color = Color.Lerp(defaultColor, defaultColor2, Random.value);
        if (_forceChange)
        {
            foreach (Renderer _r in renderers)
            {
                _r.material.DOColor(_color, "_BaseColor", 0);
            }
        }
        else
        {
            animator.SetTrigger("NeutralTrigger");
            SetFace(currentStatus);
            DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () =>
            {
                foreach (Renderer _r in renderers)
                {
                    _r.material.DOColor(_color, "_BaseColor", colorChangeDuration).SetEase(colorChangeEase);
                }
            });
        }
    }

    public void SetHappy()
    {
        if (currentStatus == SpectatorStatus.HAPPY) return;
        currentStatus = SpectatorStatus.HAPPY;
        Color _color = Color.Lerp(happyColor, happyColor2, Random.value);
        animator.SetTrigger("LaughingTrigger");
        SetFace(currentStatus);
        transform.DOKill(false);
        DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () =>
        {
            foreach (Renderer _r in renderers)
            {
                _r.material.DOColor(_color, "_BaseColor", colorChangeDuration).SetEase(colorChangeEase);
            }
        });
    }

    public void SetAngry()
    {
        if (currentStatus == SpectatorStatus.ANGRY) return;
        currentStatus = SpectatorStatus.ANGRY;
        Color _color = Color.Lerp(angryColor, angryColor2, Random.value);
        animator.SetTrigger("AngryTrigger");
        SetFace(currentStatus);
        transform.DOKill(false);
        DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () =>
        {
            foreach (Renderer _r in renderers)
            {
                _r.material.DOColor(_color, "_BaseColor", colorChangeDuration).SetEase(colorChangeEase);
            }
        });
    }

    public void SetFace(SpectatorStatus _status)
    {
        foreach (GameObject _g in happyFaces)
            _g.SetActive(false);
        foreach (GameObject _g in angryFaces)
            _g.SetActive(false);
        switch (_status) {
            case SpectatorStatus.NEUTRAL:
                break;
            case SpectatorStatus.HAPPY:
                int _randomIndex = Random.Range(0, happyFaces.Count);
                GameObject _selectedFace = happyFaces[_randomIndex];
                _selectedFace.transform.localScale = Vector3.zero;
                _selectedFace.SetActive(true);
                _selectedFace.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
                break;
            case SpectatorStatus.ANGRY:
                int _randomIndex2 = Random.Range(0, angryFaces.Count);
                GameObject _selectedFace2 = angryFaces[_randomIndex2];
                _selectedFace2.transform.localScale = Vector3.zero;
                _selectedFace2.SetActive(true);
                _selectedFace2.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
                break;
        }
    }
}
