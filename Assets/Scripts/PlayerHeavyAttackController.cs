using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHeavyAttackController : AttackController
{
    [Header("Settings")]
    [SerializeField] private int attackDamages = 10;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackLength;
    [SerializeField] private int raycastAmount;
    [SerializeField] private Transform attackSource;

    [SerializeField] private float pushForce = 1f;
    [SerializeField] private float pushDuration = 0.2f;
    [SerializeField] private Ease pushEase = Ease.OutSine;

    [SerializeField] private float dashDistance = 1f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private Ease dashEase;

    [SerializeField] private float endOfAttackSlowMultiplier = 0.2f;
    [SerializeField] private float endOfAttackSlowDuration = 1f;


    private List<Hitable> recentlyHitPawns = new List<Hitable>();
    private bool attacking = false;

    private List<string> attackStatus = new List<string>(); //Store attack status to cancel them if necessary
    private List<Tween> attackTweens = new List<Tween>(); //Same for tweens
    protected override void StartAttack()
    {
        //Reset values
        attackTweens.Clear();
        attackStatus.Clear();
        recentlyHitPawns.Clear(); //Clear recently hit pawns before starting new attack

        //Start attack
        StartDash();
    }

    public void StartDash()
    {
        attacking = true;
        string _dashStatusID;
        attackTweens.Add(linkedPawn.Push(linkedPawn.transform.forward * dashDistance, dashDuration, dashEase, out _dashStatusID));
        attackStatus.Add(_dashStatusID);
        Invoke("FinishAttack", dashDuration);
    }

    public override void CancelAttack()
    {
        CancelInvoke();
        foreach (Tween _tween in attackTweens)
        {
            if (_tween != null) _tween.Kill(false);
        }
        foreach (string _statusID in attackStatus)
        {
            linkedPawn.RemoveStatus(_statusID);
        }
        attacking = false;
    }

    private void FinishAttack()
    {
        attackStatus.Add(linkedPawn.SetStatus(new StatusEffect(StatusType.SPEED_MULTIPLIER, endOfAttackSlowDuration, endOfAttackSlowMultiplier)));
        attacking = false;
    }
    protected override void Update()
    {
        base.Update();
        if (attacking)
        {
            Vector3 _attackDirection = attackSource.forward;

            List<Hitable> _foundHitables = new List<Hitable>();
            float _angleBetweenRays = attackRadius / (float)(raycastAmount - 1);
            for (int i = 0; i < raycastAmount; i++)
            {
                Vector3 rayDirection = Quaternion.Euler(0, -attackRadius / 2 + i * _angleBetweenRays, 0) * _attackDirection;

                Ray ray = new Ray(attackSource.position, rayDirection);
                Debug.DrawRay(attackSource.position, rayDirection * attackLength, Color.red, 1f);

                foreach (RaycastHit _hit in Physics.RaycastAll(ray, attackLength))
                {
                    Hitable _foundHitable = _hit.transform.GetComponent<Hitable>();
                    if (_foundHitable != null)
                    {
                        if (_foundHitable.GetTeamID() != linkedPawn.GetTeamID() && !_foundHitables.Contains(_foundHitable) && !recentlyHitPawns.Contains(_foundHitable))
                        {
                            _foundHitables.Add(_foundHitable);
                            recentlyHitPawns.Add(_foundHitable);
                        }
                    }
                }
            }
            //Apply damages and push found targets
            foreach (Hitable _hitable in _foundHitables)
            {
                _hitable.Damage(attackDamages, DamageType.Attack);
                PawnController _foundPawn = _hitable.GetComponent<PawnController>();
                if (_foundPawn != null)
                {
                    Vector3 _pushDirection = attackSource.forward;
                    _pushDirection.y = 0;
                    _pushDirection.Normalize();
                    _foundPawn.Push(_pushDirection * pushForce, pushDuration, pushEase);
                    _foundPawn.SetStatus(new StatusEffect(StatusType.IS_BOWLING_BALL, pushDuration, 1)); //IS BOWLING BALL means the target will stun its allies nearby

                    //Cancel attacks ?
                    _foundPawn.CancelAttacks();
                }
            }
        }
    }
}
