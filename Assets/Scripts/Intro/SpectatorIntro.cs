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
                foreach (Renderer _r in renderers)
                {
                    _r.material.DOColor(Color.green, "_BaseColor", 0).SetEase(Ease.InExpo);
                }
                break;
            case "Neutral":
                animator.SetTrigger("NeutralTrigger");
                foreach (Renderer _r in renderers)
                {
                    _r.material.DOColor(Color.black, "_BaseColor", 0).SetEase(Ease.InExpo);
                }
                break;
            case "Angry":
                animator.SetTrigger("AngryTrigger");
                foreach (Renderer _r in renderers)
                {
                    _r.material.DOColor(Color.red, "_BaseColor", 0).SetEase(Ease.InExpo);
                }
                break;
        }
    }
}
