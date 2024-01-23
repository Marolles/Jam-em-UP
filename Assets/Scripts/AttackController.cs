using UnityEngine;

public abstract class AttackController : MonoBehaviour
{
    [SerializeField] protected PawnController linkedPawn;
    [SerializeField] private float cooldown;

    private float currentCD;

    public virtual void TryAttack()
    {
        if (currentCD > 0) return;
        currentCD = cooldown;
        StartAttack();
    }

    protected abstract void StartAttack();
    public abstract void CancelAttack();

    protected virtual void Update()
    {
        HandleCooldown();
    }
    private void HandleCooldown()
    {
        if (currentCD > 0)
        {
            currentCD -= Time.deltaTime;
        }
        else
        {
            currentCD = 0;
        }
    }
}
