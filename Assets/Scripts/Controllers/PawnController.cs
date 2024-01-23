using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PawnController : Hitable
{
    protected CharacterController charController;
    private Dictionary<string, StatusEffect> currentStatus = new Dictionary<string, StatusEffect>();
    private Transform lockedLookedTarget; //If true, will look this target, else HandleRotation will take over

    protected override void Awake()
    {
        base.Awake();
        charController = GetComponent<CharacterController>();
    }
    protected override void Update()
    {
        base.Update(); //The base update manages HP

        HandleStatusEffects();

        if (!isDead && !IsStunned())
        {
            HandleMovement();
            if (!lockedLookedTarget)
            {
                HandleRotation();
            } else
            {
                LookLockedTarget();
            }
            HandleAttack();
        }
    }

    public string SetStatus(StatusEffect _statusEffect)
    {
        string _statusID = UniqueIDGenerator.GenerateUniqueID();
        currentStatus[_statusID] = _statusEffect;
        return _statusID;
    }
    public void RemoveStatus(string _statusID)
    {
        if (_statusID == null) return; //StatusID is null
        if (currentStatus.ContainsKey(_statusID))
        {
            currentStatus.Remove(_statusID);
        }
    }

    public void CancelAttacks()
    {
        foreach (AttackController _ac in GetComponents<AttackController>())
        {
            _ac.CancelAttack();
        }
    }

    private void HandleStatusEffects()
    {
        List<string> _statusToRemove = new List<string>();
        //Reduce CDs
        foreach (KeyValuePair<string, StatusEffect> _kvp in currentStatus)
        {
            _kvp.Value.remainingDuration -= Time.deltaTime;
            if (_kvp.Value.remainingDuration <= 0)
            {
                _statusToRemove.Add(_kvp.Key);
            }
        }

        foreach (string _statusID in _statusToRemove)
        {
            currentStatus.Remove(_statusID);
        }
    }

    public List<StatusEffect> GetStatusEffects(StatusType _type)
    {
        List<StatusEffect> _foundStatus = new List<StatusEffect>();
        foreach (StatusEffect _se in currentStatus.Values)
        {
            if (_se.type == _type) _foundStatus.Add(_se);
        }
        return _foundStatus;
    }
    public abstract void HandleMovement();
    public abstract void HandleRotation();
    public abstract void HandleAttack();

    private void LookLockedTarget()
    {
        Vector3 _lookPos = lockedLookedTarget.position - transform.position;
        _lookPos.y = 0;
        transform.forward = _lookPos;
    }
    public float GetSpeedMultiplier()
    {
        float _multiplier = 1f;
        foreach (StatusEffect _se in GetStatusEffects(StatusType.SPEED_MULTIPLIER))
        {
            _multiplier *= _se.intensity;
        }
        return _multiplier;
    }

    public void SetLockedLookedTarget(Transform _transform)
    {
        lockedLookedTarget = _transform;
    }

    public void RemoveLockedLookedTarget()
    {
        lockedLookedTarget = null;
    }

    public bool IsStunned()
    {
        if (GetStatusEffects(StatusType.STUN).Count > 0) return true;
        return false;
    }

    public Tween Push(Vector3 _direction, float _pushDuration, Ease _ease)
    {
        string _temp;
        return Push(_direction, _pushDuration, _ease, out _temp);
    }

    public Tween Push(Vector3 _direction, float _pushDuration, Ease _ease, out string _statusID)
    {
        _statusID = SetStatus(new StatusEffect(StatusType.STUN, _pushDuration, 1f));
        return transform.DOMove(transform.position + _direction, _pushDuration).SetEase(_ease);
    }

    public override void Kill(DamageType _fatalDamageType)
    {
        CancelAttacks();
        base.Kill(_fatalDamageType);
    }
}
