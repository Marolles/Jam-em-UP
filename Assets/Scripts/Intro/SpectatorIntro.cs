using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

public class SpectatorIntro : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private Renderer[] renderers;
    public string state;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color defaultColor2;

    [SerializeField] private Color happyColor;
    [SerializeField] private Color happyColor2;

    [SerializeField] private Color angryColor;
    [SerializeField] private Color angryColor2;

    // Start is called before the first frame update
    void Start()
    {
        SetState(state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetState(string _state)
    {
        switch (_state)
        {
            case "Laughing":
                animator.SetTrigger("LaughingTrigger");
                Color _color = Color.Lerp(happyColor, happyColor2, Random.value);
                foreach (Renderer _r in renderers)
                {
                    _r.material.DOColor(_color, "_BaseColor", 0).SetEase(Ease.InExpo);
                }
                break;
            case "Neutral":
                Color _color2 = Color.Lerp(defaultColor, defaultColor2, Random.value);
                animator.SetTrigger("NeutralTrigger");
                foreach (Renderer _r in renderers)
                {
                    _r.material.DOColor(_color2, "_BaseColor", 0).SetEase(Ease.InExpo);
                }
                break;
            case "Angry":
                Color _color3 = Color.Lerp(angryColor, angryColor2, Random.value);
                animator.SetTrigger("AngryTrigger");
                foreach (Renderer _r in renderers)
                {
                    _r.material.DOColor(_color3, "_BaseColor", 0).SetEase(Ease.InExpo);
                }
                break;
        }
    }
}
