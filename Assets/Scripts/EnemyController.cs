using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Hitable
{
    [Header("Movement settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private CharacterController charController;
    private Vector3 wantedForward;

    protected override void Awake()
    {
        base.Awake();
        charController = GetComponent<CharacterController>();
    }
    protected override void Update()
    {
        base.Update(); //Handle HPs
        if (!isDead)
        {
            HandleMovement();
            HandleRotation();
        }
    }


    private void HandleMovement()
    {
        Transform _currentTarget = PlayerController.instance.transform;
        if (_currentTarget != null)
        {
            Vector3 _moveDirection = (_currentTarget.position - transform.position).normalized;
            charController.Move(_moveDirection * Time.deltaTime * moveSpeed);
        }
    }

    private void HandleRotation()
    {
        Transform _currentTarget = PlayerController.instance.transform;
        if (_currentTarget != null)
        {
            wantedForward = _currentTarget.transform.position - transform.position;
            wantedForward.y = 0;
        }
        transform.forward = Vector3.Lerp(transform.forward, wantedForward, Time.deltaTime * rotationSpeed);
    }
    public void Push(Vector3 _direction, float _pushDuration, Ease _ease)
    {
        transform.DOMove(transform.position + _direction, _pushDuration).SetEase(_ease);
       // charController.SimpleMove(_direction);
    }
}
