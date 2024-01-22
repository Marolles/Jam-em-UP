using UnityEngine;

public class EnemyController : PawnController
{
    [Header("Movement settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private Vector3 wantedForward;

    protected override void Awake()
    {
        base.Awake();
        WaveManager.currentEnemies.Add(this); //Register the enemy to the wave manager, to track their amount
    }
    public override void HandleMovement()
    {
        Transform _currentTarget = PlayerController.instance.transform;
        if (_currentTarget != null)
        {
            Vector3 _moveDirection = (_currentTarget.position - transform.position).normalized;
            if (charController.enabled)
                charController.Move(_moveDirection * Time.deltaTime * moveSpeed);
        }
    }

    public override void HandleRotation()
    {
        Transform _currentTarget = PlayerController.instance.transform;
        if (_currentTarget != null)
        {
            wantedForward = _currentTarget.transform.position - transform.position;
            wantedForward.y = 0;
        }
        transform.forward = Vector3.Lerp(transform.forward, wantedForward, Time.deltaTime * rotationSpeed);
    }

    public override void HandleAttack()
    {
        //No attack yet :(
    }

    public override void Kill()
    {
        base.Kill();
        if (WaveManager.currentEnemies.Contains(this))
            WaveManager.currentEnemies.Remove(this); //Unregister the enemy from the wave manager since it is dead
    }
}
