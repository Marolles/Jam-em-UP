using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachedArmorPart : MonoBehaviour
{
    public const float DELAY_BEFORE_DESTROYING = 4;

    private void Awake()
    {
        Invoke("Clean", DELAY_BEFORE_DESTROYING);
    }

    private void Clean()
    {
        transform.DOScale(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => Destroy(this.gameObject));
    }

}
