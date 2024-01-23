using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TickleController : AttackController
{
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackLength;
    [SerializeField] private int raycastAmount;
    [SerializeField] private Transform attackSource;

    [SerializeField] private float ticklingDuration = 1f;
    [SerializeField] private float ticklingSlowMultiplier = 0.1f;

    [SerializeField] private float maxTicklingDistance = 3f;
    private List<string> attackStatus = new List<string>();
    private List<Tween> attackTweens = new List<Tween>();

    private PawnController tickleTarget;
    private string tickleTargetStatusID;

    public override void CancelAttack()
    {
        linkedPawn.RemoveLockedLookedTarget();
        CancelInvoke();

        //Cancel animations
        foreach (Tween _tween in attackTweens)
        {
            _tween?.Kill(false);
        }

        //Cancel attacker status
        foreach (string _statusID in attackStatus)
        {
            linkedPawn.RemoveStatus(_statusID);
        }

        //Unstun target
        if (tickleTarget != null && tickleTargetStatusID != null)
        {
            tickleTarget.RemoveStatus(tickleTargetStatusID);
        }
        tickleTarget = null;
        tickleTargetStatusID = null;
    }

    protected override void StartAttack()
    {
        //Reset values
        attackTweens.Clear();
        attackStatus.Clear();

        PawnController _foundTarget = GetNearestTarget();
        if (_foundTarget != null)
        {
            tickleTarget = _foundTarget;
            linkedPawn.SetLockedLookedTarget(_foundTarget.transform);

            //Slow attacker
            attackStatus.Add(linkedPawn.SetStatus(new StatusEffect(StatusType.SPEED_MULTIPLIER, ticklingDuration, ticklingSlowMultiplier)));

            //Cancel target attacks
            tickleTarget.CancelAttacks();

            //Stun target
            tickleTarget.SetStatus(new StatusEffect(StatusType.STUN, ticklingDuration, 1));

            Invoke("FinishTickle", ticklingDuration);
        }
    }

    private void FinishTickle()
    {
        tickleTarget.Damage(999, DamageType.Tickling);
    }

    protected override void Update()
    {
        base.Update();
        if (tickleTarget != null) //If enemy is being tickled, check its distance
        {
            if (Vector3.Distance(tickleTarget.transform.position, linkedPawn.transform.position) > maxTicklingDistance)
            {
                //Tickle distance is exceeded
                CancelAttack();
            }
        }
    }

    private PawnController GetNearestTarget()
    {
        Vector3 _attackDirection = attackSource.forward;
        List<PawnController> _foundHitables = new List<PawnController>();

        float _angleBetweenRays = attackRadius / (float)(raycastAmount - 1);
        for (int i = 0; i < raycastAmount; i++)
        {
            Vector3 rayDirection = Quaternion.Euler(0, -attackRadius / 2 + i * _angleBetweenRays, 0) * _attackDirection;

            Ray ray = new Ray(attackSource.position, rayDirection);
            Debug.DrawRay(attackSource.position, rayDirection * attackLength, Color.red, 1f);

            foreach (RaycastHit _hit in Physics.RaycastAll(ray, attackLength))
            {
                PawnController _foundHitable = _hit.transform.GetComponent<PawnController>();
                if (_foundHitable != null)
                {
                    if (_foundHitable.GetTeamID() != linkedPawn.GetTeamID() && !_foundHitable.IsDead() && !_foundHitable.HasShield() && !_foundHitables.Contains(_foundHitable))
                    {
                        _foundHitables.Add(_foundHitable);
                    }
                }
            }

        }

        //Get nearest hitable to forward direction
        PawnController _nearestHitable = null;
        float _smallestDistance = Mathf.Infinity;
        foreach (PawnController _hitable in _foundHitables)
        {
            Vector3 _targetDirection = _hitable.transform.position - attackSource.position;
            _targetDirection.y = 0;
            float _distance = Vector3.Distance(attackSource.position, _hitable.transform.position);
            if(_distance < _smallestDistance)
            {
                _nearestHitable = _hitable;
                _smallestDistance = _distance;
            }
        }
        return _nearestHitable;
    }
}
