using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TickleController : AttackController
{
    [SerializeField] private GameObject rightClickHintPrefab;

    [SerializeField] private float attackRadius;
    [SerializeField] private float attackLength;
    [SerializeField] private int raycastAmount;
    [SerializeField] private Transform attackSource;

    [SerializeField] private float ticklingDuration = 1f;
    [SerializeField] private float ticklingSlowMultiplier = 0.1f;

    [SerializeField] private float maxTicklingDistance = 3f;
    private List<string> attackStatus = new List<string>();
    private List<Tween> attackTweens = new List<Tween>();

    private PawnController nearestTarget;

    private PawnController tickleTarget;
    private string tickleTargetStatusID;
    private GameObject rightClick_hint;


    private void Awake()
    {
        rightClick_hint = Instantiate(rightClickHintPrefab, MainCanvas.instance.transform);
        rightClick_hint.SetActive(false);
    }
    public override void CancelAttack()
    {
        //Animator
        linkedPawn.GetAnimator().SetBool("TickleBool", false);
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
        if (tickleTarget != null)
        {
            tickleTarget.GetAnimator().SetBool("TickledBoolean", false);
            if (tickleTargetStatusID != null)
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

        if (nearestTarget != null)
        {
            tickleTarget = nearestTarget;
            linkedPawn.SetLockedLookedTarget(nearestTarget.transform);

            //Animator
            linkedPawn.GetAnimator().SetBool("TickleBool", true);
            tickleTarget.GetAnimator().SetBool("TickledBoolean", true);

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
        PawnController _tickleTarget = tickleTarget;
        CancelAttack();
        _tickleTarget.Damage(999, DamageType.Tickling, attackSource.position);
    }

    protected override void Update()
    {
        base.Update();

        nearestTarget = GetNearestTarget();
        if (nearestTarget != null && nearestTarget != tickleTarget) //If has target and target isn't attacked yet, draw RIGHT CLICK over it
        {
            rightClick_hint.SetActive(true);
            rightClick_hint.transform.position = Camera.main.WorldToScreenPoint(nearestTarget.transform.position + Vector3.up * 3f);
        } else
        {
            rightClick_hint.SetActive(false);
        }

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
            //Debug.DrawRay(attackSource.position, rayDirection * attackLength, Color.red, 1f);

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
