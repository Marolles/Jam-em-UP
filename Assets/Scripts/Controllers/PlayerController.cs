using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : PawnController
{
    public static PlayerController instance;

    [Header("References")]
    [SerializeField] private PlayerHitFeedback playerHitFeedback;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 7f;

    [SerializeField] private float rotationSpeed = 5f;

    [Header("Attack Settings")]
    [SerializeField] private float leftClickDurationToTriggerHeavyAttack = 0.2f;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnRadiusExplosion = 10;
    [SerializeField] private float respawnPushForce = 10;
    [SerializeField] private float respawnPushDuration = 0.2f;
    [SerializeField] private Ease respawnPushEase = Ease.InSine;

    [Header("Fame Settings")]
    [SerializeField] private int fameLostOnDeath = 30;


    private float currentSpeed = 0f;

    private Plane groundPlane;
    private Vector3 wantedLookedPos;

    private float leftClickDuration = 0f;

    protected override void Awake()
    {
        base.Awake();
        if (instance != null) { Debug.LogError("Can't have 2 players at the same time, destroying first one."); Destroy(instance.gameObject); }
        instance = this;

        groundPlane = new Plane(Vector3.up, Vector3.zero);//Generate a virtual plane at Y = 0 to check for mouse raycasts
    }

    protected override void Update()
    {
        //Dash is here because it can be casted even while stunned
        if (Input.GetKeyDown(KeyCode.Space) && !isDead && !ArenaController.IsFrozen())
        {
                GetComponent<DashController>().TryAttack();
        }
        base.Update();
    }

    public override void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            leftClickDuration = 0;
        }
        if (Input.GetMouseButton(0))
        {
            leftClickDuration += Time.deltaTime;
            if (leftClickDuration >= leftClickDurationToTriggerHeavyAttack)
            {
                GetComponent<PlayerHeavyAttackController>().TryAttack();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (leftClickDuration < leftClickDurationToTriggerHeavyAttack)
                GetComponent<LightAttackController>().TryAttack();
        }
        if (Input.GetMouseButtonDown(1))
        {
            GetComponent<TickleController>().TryAttack();
        }
        if (Input.GetMouseButtonUp(1))
        {
            GetComponent<TickleController>().CancelAttack();
        }
    }

    public override void HandleMovement()
    {
        Vector3 _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Adapt the square mapping into a circular mapping (prevent diagonal faster movement, but keep joystick nuance)
        _movement.x *= Mathf.Sqrt(1f - (_movement.z * _movement.z / 2f));
        _movement.z *= Mathf.Sqrt(1f - (_movement.x * _movement.x / 2f));

        movementVector = _movement;

        if (_movement.magnitude > 0) //Handle acceleration if player is moving
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * acceleration);
        }
        else
        { //Else, handle deceleration if player is not moving
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * deceleration);
        }

        //Apply movement to characterController
        Vector3 _deltaMovement = _movement * Time.deltaTime * (currentSpeed * GetSpeedMultiplier());
        _deltaMovement -= Vector3.up * 9.81f * Time.deltaTime;

        Vector3 _newPosition = MapManager.ClampPositionInRadius(transform.position + _deltaMovement);
        if (charController.enabled)
            charController.Move(_newPosition - transform.position);

        //Update animator
        float _finalMovementSpeed = (currentSpeed * GetSpeedMultiplier()) * _movement.magnitude;
        animator.SetFloat("Blend", _finalMovementSpeed / moveSpeed);
    }

    public override void HandleRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            //Get the point under the mouse
            Vector3 _lookedPoint = ray.GetPoint(rayDistance);
            wantedLookedPos = _lookedPoint;
        }

        Vector3 _newDirection = wantedLookedPos - transform.position;
        _newDirection.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, _newDirection, Time.deltaTime * rotationSpeed);
    }

    public override void Damage(int _damages, DamageType _type, Vector3 _hitDirection)
    {
        base.Damage(_damages, _type, _hitDirection);
       // playerHitFeedback.PlayFeedback();
    }

    public override void Kill(DamageType _fatalDamageType)
    {
        animator.SetTrigger("DeathTrigger");

        //If the player dies, everyone is stunned on the map
        base.Kill(_fatalDamageType);
        FameController.DecreaseFameValue(fameLostOnDeath);
        ArenaController.FreezeArena();
        ArenaController.instance.AskKingForMercy();
    }
    public override void Regenerate()
    {
        base.Regenerate();

        //Animator
        animator.SetTrigger("RegenerateTrigger");

        //Push entities around
        foreach (PawnController _pc in GetPawnsInRadius(respawnRadiusExplosion))
        {
            Vector3 _pushDirection = _pc.transform.position - transform.position;
            _pushDirection.Normalize();
            _pc.Push(_pushDirection * respawnPushForce, respawnPushDuration, respawnPushEase);
        }
    }
}
