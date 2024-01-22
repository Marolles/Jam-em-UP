using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    [SerializeField] private Animator playerHitVignetteAnimator;
    public void PlayFeedback()
    {
        print("Aouch");
        playerHitVignetteAnimator.SetTrigger("PlayerHitTrigger");
    }

}
