using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool displayHPBar;
    [SerializeField] private int maxHP;

    [SerializeField] private GameObject hpBarPrefab;

    private int currentHP;
    private HPBar hpBar;


    private void Awake()
    {
        Regenerate();
        if (displayHPBar)
        {
            GameObject _hpBarObj = Instantiate(hpBarPrefab);
            hpBar = _hpBarObj.GetComponent<HPBar>();
            if (hpBar == null) { Debug.LogError("No HP bar script attached on hpbar prefab");} else
            {
                hpBar.SetFollowedTransform(transform);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage(10);
        }
    }

    public void Damage(int _damages)
    {
        currentHP = Mathf.Clamp(currentHP - _damages, 0, maxHP);
        if (hpBar != null)
        {
            hpBar.UpdateBar(GetHPPercent());
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
}
