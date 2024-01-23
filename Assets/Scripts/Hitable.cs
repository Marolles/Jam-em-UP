using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private TeamID teamID;
    [SerializeField] private float destroyAfterDelay = -1;
    [SerializeField] private bool displayHPBar;
    [SerializeField] private bool hideHPBarWhenDead = false;
    [SerializeField] private int maxHP;
    [SerializeField] private float hpBarVerticalOffset = 3f;
    [SerializeField] private int shieldPoints = 3;

    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private List<Renderer> renderers; //Temporary feedback

    private Tween colorFeedbackTween;

    private int currentHP;
    private HPBar hpBar;
    private int currentShieldPoints;

    protected bool isDead = false;


    protected virtual void Awake()
    {
        Regenerate();
        if (displayHPBar)
        {
            Canvas _mainCanvas = MainCanvas.GetCanvas();
            if (_mainCanvas != null)
            {
                GameObject _hpBarObj = Instantiate(hpBarPrefab, _mainCanvas.transform);
                hpBar = _hpBarObj.GetComponent<HPBar>();
                if (hpBar == null) { Debug.LogError("No HP bar script attached on hpbar prefab"); }
            }
        }
    }

    protected virtual void Update()
    {
        if (hpBar != null)
        {
            MoveHPBar(transform.position + Vector3.up * hpBarVerticalOffset);
        }
    }

    private void MoveHPBar(Vector3 _worldPosition)
    {
        hpBar.transform.position = Camera.main.WorldToScreenPoint(_worldPosition);
    }

    private bool TankHitWithShield()
    {
        if (currentShieldPoints <= 0) return false;

        //Shield hit feedback HERE
        colorFeedbackTween.Kill(true);
        foreach (Renderer _r in renderers)
            colorFeedbackTween = _r.material.DOColor(Color.white, 0.1f).SetLoops(2, LoopType.Yoyo);

        GameObject _shieldFXPrefab = Instantiate(Resources.Load<GameObject>("FX/ShieldHitFX"));
        _shieldFXPrefab.transform.SetParent(transform);
        _shieldFXPrefab.transform.position = transform.position + Vector3.up;

        currentShieldPoints--;

        return true;
    }
    public virtual void Damage(int _damages, DamageType _type)
    {
        if (isDead) return;

        if (TankHitWithShield()) { return; }

        currentHP = Mathf.Clamp(currentHP - _damages, 0, maxHP);

        if (currentHP <= 0) { Kill(_type); return; }

        //Damage feedback
        colorFeedbackTween.Kill(true);
        foreach (Renderer _r in renderers)
            colorFeedbackTween = _r.material.DOColor(Color.white, 0.1f).SetLoops(2, LoopType.Yoyo);
        if (hpBar != null)
        {
            hpBar.UpdateBar(GetHPPercent());
        }
    }

    public bool HasShield()
    {
        if (currentShieldPoints > 0) return true;
        return false;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public virtual void Kill(DamageType _fatalDamageType)
    {
        isDead = true;

        //Kill feedback
        colorFeedbackTween.Kill(true);
        foreach (Renderer _r in renderers)
            colorFeedbackTween = _r.material.DOColor(Color.white, 0.1f);
        if (hpBar != null)
        {
            hpBar.UpdateBar(0);
            if (hideHPBarWhenDead)
            {
                hpBar.HideHPBar(0.2f);
            }
        }

        if (destroyAfterDelay >= 0)
        {
            Invoke("Delete", destroyAfterDelay);
        }
    }
    public void Regenerate()
    {
        currentHP = maxHP;
        currentShieldPoints = shieldPoints;
    }

    public int GetHP()
    {
        return currentHP;
    }

    public float GetHPPercent()
    {
        return (float)currentHP / (float)maxHP;
    }

    public TeamID GetTeamID()
    {
        return teamID;
    }

    public virtual void Delete()
    {
        if (hpBar != null) { Destroy(hpBar.gameObject); }
        Destroy(this.gameObject);
    }
}
