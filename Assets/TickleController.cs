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

    public override void CancelAttack()
    {

    }

    protected override void StartAttack()
    {
        Hitable _foundTarget = GetNearestTarget();
        _foundTarget.Kill(); //DEBUG, TEMPORARY
    }

    private Hitable GetNearestTarget()
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
                    if (_foundHitable.GetTeamID() != linkedPawn.GetTeamID() && !_foundHitables.Contains(_foundHitable))
                    {
                        _foundHitables.Add(_foundHitable);

                    }
                }
            }

        }

        //Get nearest hitable to forward direction
        Hitable _nearestHitable = null;
        float _smallestAngle = Mathf.Infinity;
        foreach (Hitable _hitable in _foundHitables)
        {
            Vector3 _targetDirection = _hitable.transform.position - attackSource.position;
            _targetDirection.y = 0;
            float _angle = Vector3.SignedAngle(_attackDirection, _targetDirection, Vector3.up);
            if( _angle < _smallestAngle)
            {
                _nearestHitable = _hitable;
                _smallestAngle = _angle;
            }
        }
        return _nearestHitable;
    }
}
