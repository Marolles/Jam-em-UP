using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private TeamID teamID;
    [SerializeField] private bool displayHPBar;
    [SerializeField] private int maxHP;
    [SerializeField] private float hpBarVerticalOffset = 3f;

    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private List<Renderer> renderers; //Temporary feedback

    private int currentHP;
    private HPBar hpBar;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage(10);
        }
        if (hpBar != null)
        {
            MoveHPBar(transform.position + Vector3.up * hpBarVerticalOffset);
        }
    }

    private void MoveHPBar(Vector3 _worldPosition)
    {
        hpBar.transform.position = Camera.main.WorldToScreenPoint(_worldPosition);
    }

    public void Damage(int _damages)
    {
        if (isDead) return;
        currentHP = Mathf.Clamp(currentHP - _damages, 0, maxHP);

        if (currentHP <= 0) { Kill(); return; }

        //Damage feedback
        foreach (Renderer _r in renderers)
            _r.material.DOColor(Color.white, 0.1f).SetLoops(2, LoopType.Yoyo);
        if (hpBar != null)
        {
            hpBar.UpdateBar(GetHPPercent());
        }
    }

    public void Kill()
    {
        isDead = true;

        //Kill feedback
        foreach (Renderer _r in renderers)
            _r.material.DOColor(Color.white, 0.1f);
        if (hpBar != null)
        {
            hpBar.UpdateBar(0);
        }
    }
    public void Regenerate()
    {
        currentHP = maxHP;
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
}
