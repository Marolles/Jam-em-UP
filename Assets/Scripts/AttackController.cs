using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TeamID attackerTeam;
    [SerializeField] private int attackDamages = 10;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackLength;
    [SerializeField] private int raycastAmount;
    [SerializeField] private Transform attackSource;
    [SerializeField] private float pushDistance = 1f;
    [SerializeField] private float pushDuration = 0.2f;
    [SerializeField] private Ease pushEase = Ease.OutSine;

    public void Attack()
    {
        //Checks for 'hitable' in front of the attacksource, at a certain angle, and store them in a list
        List<Hitable> _hitTargets = new List<Hitable>();
        Vector3 _attackDirection = attackSource.forward;

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
                    if (_foundHitable.GetTeamID() != attackerTeam && !_hitTargets.Contains(_foundHitable))
                    {
                        _hitTargets.Add(_foundHitable);
                    }
                }
            }
        }

        //Apply damages and push found targets
        foreach (Hitable _hitable in _hitTargets)
        {
            _hitable.Damage(attackDamages);
            PawnController _foundPawn = _hitable.GetComponent<PawnController>();
            if (_foundPawn != null)
            {
                _foundPawn.Push(attackSource.forward * pushDistance, pushDuration, pushEase);
            }
        }
    }
}
