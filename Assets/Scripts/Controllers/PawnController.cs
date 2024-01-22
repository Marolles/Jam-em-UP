using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PawnController : Hitable
{
    protected CharacterController charController;
    private float currentStunDuration;
    private Dictionary<float, float> slowModifiers = new Dictionary<float, float>();

    protected override void Awake()
    {
        base.Awake();
        charController = GetComponent<CharacterController>();
    }
    protected override void Update()
    {
        base.Update(); //The base update manages HP

        HandleStunDuration();

        if (!isDead && currentStunDuration <= 0)
        {
            HandleMovement();
            HandleRotation();
            HandleAttack();
        }
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
    public void Push(Vector3 _direction, float _pushDuration, Ease _ease)
    {
        if (HasShield()) return; //Can't push if shield is active
        Stun(_pushDuration);
        transform.DOMove(transform.position + _direction, _pushDuration).SetEase(_ease);
    }
}
