using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PawnController : Hitable
{
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
}
