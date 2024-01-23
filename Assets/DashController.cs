using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : AttackController
{
    [SerializeField] private float dashDistance = 5;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private Ease dashEase = Ease.OutSine;

    private List<string> attackStatus = new List<string>(); //Store attack status to cancel them if necessary
    private List<Tween> attackTweens = new List<Tween>(); //Same for tweens

    public override void CancelAttack()
    {
        foreach (Tween _tween in attackTweens)
        {
            if (_tween != null) _tween.Kill(false);
        }
        foreach (string _statusID in attackStatus)
        {
            linkedPawn.RemoveStatus(_statusID);
        }
    }

    protected override void StartAttack()
    {
        //Clear variables
        attackTweens.Clear();
        attackStatus.Clear();

        linkedPawn.CancelAttacks();
        string _dashStatusID;
        Vector3 _movementVector = linkedPawn.GetMovementVector().normalized;
        _movementVector.y = 0;
        if (_movementVector.magnitude == 0) _movementVector = transform.forward;
        attackTweens.Add(linkedPawn.Push(_movementVector * dashDistance, dashDuration, dashEase, out _dashStatusID));
        attackStatus.Add(_dashStatusID);

        //Animator
        linkedPawn.GetAnimator().SetTrigger("DashTrigger");
    }
}
