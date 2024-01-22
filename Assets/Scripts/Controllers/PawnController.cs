using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PawnController : Hitable
{
    protected CharacterController charController;
    private float currentStunDuration;
    List<KeyValuePair<float, float>> speedModifiers = new List<KeyValuePair<float, float>>();

    protected override void Awake()
    {
        base.Awake();
        charController = GetComponent<CharacterController>();
    }
    protected override void Update()
    {
        base.Update(); //The base update manages HP

        HandleStunDuration();
        HandleSpeedModifiers();

        if (!isDead && currentStunDuration <= 0)
        {
            HandleMovement();
            HandleRotation();
            HandleAttack();
        }
    }

    private void HandleSpeedModifiers()
    {
        List<KeyValuePair<float, float>> _updatedSpeedModifiers = new List<KeyValuePair<float, float>>();
        for (int i = 0; i < speedModifiers.Count; i++)
        {
            KeyValuePair<float, float> _newValue = new KeyValuePair<float, float>(speedModifiers[i].Key - Time.deltaTime, speedModifiers[i].Value);
            if (_newValue.Key > 0)
            {
                _updatedSpeedModifiers.Add(_newValue);
            }
        }
        speedModifiers = _updatedSpeedModifiers;
    }

    private void HandleStunDuration()
    {
        if (currentStunDuration > 0)
        {
            currentStunDuration -= Time.deltaTime;
        } else
        {
            currentStunDuration = 0;
        }
    }
    public abstract void HandleMovement();
    public abstract void HandleRotation();
    public abstract void HandleAttack();

    public void Stun(float _duration)
    {
        if (currentStunDuration < _duration)
        {
            currentStunDuration = _duration;
        }
    }

    public float GetSpeedMultiplier()
    {
        float _multiplier = 1f;
        foreach (KeyValuePair<float, float> _kvp in speedModifiers)
        {
            _multiplier *= _kvp.Value;
        }
        return _multiplier;
    }

    public void SetSpeedMultiplier(float _duration, float _multiplier)
    {
        speedModifiers.Add(new KeyValuePair<float, float>(_duration, _multiplier));
    }
    public void Push(Vector3 _direction, float _pushDuration, Ease _ease)
    {
        if (HasShield()) return; //Can't push if shield is active
        Stun(_pushDuration);
        transform.DOMove(transform.position + _direction, _pushDuration).SetEase(_ease);
    }
}
