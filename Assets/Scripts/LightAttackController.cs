using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LightAttackController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PawnController linkedPawn;
    [SerializeField] private int attackDamages = 10;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackLength;
    [SerializeField] private int raycastAmount;
    [SerializeField] private Transform attackSource;
    [SerializeField] private float pushDistance = 1f;
    [SerializeField] private float pushDuration = 0.2f;
    [SerializeField] private Ease pushEase = Ease.OutSine;

    [SerializeField] private float dashDistance = 1f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private Ease dashEase;


    private List<Hitable> recentlyHitPawns = new List<Hitable>();
    private bool attacking = false;
    public void Attack()
    {
        attacking = true;
        recentlyHitPawns.Clear(); //Clear recently hit pawns before starting new attack
        linkedPawn.Push(linkedPawn.transform.forward * dashDistance, dashDuration, dashEase);
        Invoke("FinishAttack", dashDuration);
    }

    private void FinishAttack()
    {
        //Apply damages and push found targets
        foreach (Hitable _hitable in recentlyHitPawns)
        {
            _hitable.Damage(attackDamages);
            PawnController _foundPawn = _hitable.GetComponent<PawnController>();
            if (_foundPawn != null)
            {
                Vector3 _pushDirection = _foundPawn.transform.position - transform.position;
                _pushDirection.y = 0;
                _pushDirection.Normalize();
                _foundPawn.Push(_pushDirection * pushDistance, pushDuration, pushEase);
            }
        }
    }

    private void Update()
    {
        if (attacking)
        {
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
                        if (_foundHitable.GetTeamID() != linkedPawn.GetTeamID() && !recentlyHitPawns.Contains(_foundHitable))
                        {
                            recentlyHitPawns.Add(_foundHitable);
                        }
                    }
                }
            }
        }
    }
}
