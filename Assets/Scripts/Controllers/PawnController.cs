using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PawnController : Hitable
{
    protected CharacterController charController;
    protected override void Awake()
    {
        base.Awake();
        charController = GetComponent<CharacterController>();
    }
    protected override void Update()
    {
        base.Update(); //The base update manages HP

        if (!isDead)
        {
            HandleMovement();
            HandleRotation();
            HandleAttack();
        }
    }

    public abstract void HandleMovement();
    public abstract void HandleRotation();
    public abstract void HandleAttack();
    public void Push(Vector3 _direction, float _pushDuration, Ease _ease)
    {
        charController.enabled = false;
        transform.DOMove(transform.position + _direction, _pushDuration).SetEase(_ease).OnComplete(() => charController.enabled = true);
    }
}
