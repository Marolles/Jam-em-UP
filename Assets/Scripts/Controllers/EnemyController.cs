using Autodesk.Fbx;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyController : PawnController
{
    [Header("Movement settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float distanceToAttack = 3f;

    [SerializeField] private float fleeingDistance = 10f;
    [SerializeField] private float fleeingSpeedMulitplier = 0.5f;
    [SerializeField] private float fleeMoveSpeedDuration = 2f;
    [SerializeField] private float fleeMoveSpeedMultiplier = 3f;

    [SerializeField] private float stunDurationWhenShieldDown = 1f;
    private Vector3 wantedForward;

    protected override void Awake()
    {
        base.Awake();
        WaveManager.currentEnemies.Add(this); //Register the enemy to the wave manager, to track their amount
        onShieldExplosion.AddListener(ShieldExplosion);
    }

    protected override void Update()
    {
        base.Update();
    }
    public override void HandleMovement()
    {
        Transform _currentTarget = PlayerController.instance.transform;
        Vector3 _moveDirection = Vector3.zero;
        float _speedMultiplier = GetSpeedMultiplier();
        if (_currentTarget != null)
        {
            if (HasShield())
            {
                //If shield, then move toward the target
                _moveDirection = (_currentTarget.position - transform.position).normalized;
            }
            else
            {
                //If no shield, then flee the target
                _speedMultiplier *= fleeingSpeedMulitplier;
                if (Vector3.Distance(_currentTarget.position, transform.position) < fleeingDistance)
                    _moveDirection = (transform.position - _currentTarget.position ).normalized;
            }
        }
        Vector3 _deltaMovement = _moveDirection * Time.deltaTime * (moveSpeed * _speedMultiplier);
        _deltaMovement -= Vector3.up * 9.81f * Time.deltaTime;

        Vector3 _newPosition = MapManager.ClampPositionInRadius(transform.position + _deltaMovement);
        if (charController.enabled)
            charController.Move(_newPosition - transform.position);
    }

    public override void HandleRotation()
    {
        Transform _currentTarget = PlayerController.instance.transform;
        if (_currentTarget != null)
        {
            if (HasShield())
            {
                wantedForward = _currentTarget.transform.position - transform.position;
            }
            else
            {
                wantedForward = transform.position - _currentTarget.transform.position;
            }
        }
        wantedForward.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, wantedForward, Time.deltaTime * rotationSpeed);
    }

    public override void HandleAttack()
    {
        if (!HasShield()) return; //No shield = no attack
        float _distanceToPlayer = Vector3.Distance(PlayerController.instance.transform.position, transform.position);
        if (_distanceToPlayer < distanceToAttack)
        {
            GetComponent<HeavyAttackController>().TryAttack();
        }
    }

    public override void Kill(DamageType _fatalDamageType)
    {
        animator.ResetTrigger("HeavyAttackTrigger");
        animator.ResetTrigger("NoArmorTrigger");
        animator.ResetTrigger("HitTrigger");

        base.Kill(_fatalDamageType);
        if (WaveManager.currentEnemies.Contains(this))
            WaveManager.currentEnemies.Remove(this); //Unregister the enemy from the wave manager since it is dead, normally enemy is already unregistered when losing its armor

        switch (_fatalDamageType)
        {
            case DamageType.Attack:
                animator.SetTrigger("DeathSwordTrigger");
                FameController.DecreaseFameValue(FameController.instance.fameLostOnKill);
                ScoreBoard.instance.IncreaseScore(ScoreBoard.instance.scoreOnKill);
                break;
            case DamageType.Tickling:
                animator.SetTrigger("DeathTickleTrigger");
                FameController.IncreaseFameValue(FameController.instance.fameGainedOnTickle);
                ScoreBoard.instance.IncreaseScore(ScoreBoard.instance.scoreOnTickle);
                break;
        }
    }

    private void ShieldExplosion()
    {
        SetStatus(new StatusEffect(StatusType.STUN, stunDurationWhenShieldDown, 1));
        CancelAttacks();
        GetAnimator().SetTrigger("NoArmorTrigger");
        SetStatus(new StatusEffect(StatusType.SPEED_MULTIPLIER, fleeMoveSpeedDuration, fleeMoveSpeedMultiplier));

        //Unregister from wave manager
        if (WaveManager.currentEnemies.Contains(this))
            WaveManager.currentEnemies.Remove(this); //Unregister the enemy from the wave manager since it is dead
    }
}
