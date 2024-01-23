using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeavyAttackController : AttackController
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

    [SerializeField] private float anticipationSlowMultiplier = 0.2f;
    [SerializeField] private float anticipationDuration = 1f;
    [SerializeField] private float anticipationPercentTrackingPlayer = 0.5f;

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

        //Start anticipation
        float _anticipationFirstPartDuration = anticipationDuration * anticipationPercentTrackingPlayer;
        attackStatus.Add(linkedPawn.SetStatus(new StatusEffect(StatusType.SPEED_MULTIPLIER, _anticipationFirstPartDuration, anticipationSlowMultiplier)));
        Invoke("AnticipationSecondPart", _anticipationFirstPartDuration);
    }

    private void AnticipationSecondPart()
    {
        float _anticipationSecondPartDuration = anticipationDuration * (1f - anticipationPercentTrackingPlayer);
        attackStatus.Add(linkedPawn.SetStatus(new StatusEffect(StatusType.STUN, _anticipationSecondPartDuration + 0.1f, 1)));
        Invoke("StartDash", _anticipationSecondPartDuration);
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
                    Vector3 _pushDirection = _foundPawn.transform.position - transform.position;
                    _pushDirection.y = 0;
                    _pushDirection.Normalize();
                    _foundPawn.Push(_pushDirection * pushForce, pushDuration, pushEase);

                    //Cancel attacks
                    _foundPawn.CancelAttacks();
                }
            }
        }
    }
}
