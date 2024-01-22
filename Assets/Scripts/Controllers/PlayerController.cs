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

    private float currentSpeed = 0f;

    private Plane groundPlane;
    private Vector3 wantedLookedPos;

    protected override void Awake()
    {
        base.Awake();
        if (instance != null) { Debug.LogError("Can't have 2 players at the same time, destroying first one."); Destroy(instance.gameObject); }
        instance = this;

        groundPlane = new Plane(Vector3.up, Vector3.zero);//Generate a virtual plane at Y = 0 to check for mouse raycasts
    }

    public override void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<LightAttackController>().Attack();
        }
    }

    public override void HandleMovement()
    {
        Vector3 _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Adapt the square mapping into a circular mapping (prevent diagonal faster movement, but keep joystick nuance)
        _movement.x *= Mathf.Sqrt(1f - (_movement.z * _movement.z / 2f));
        _movement.z *= Mathf.Sqrt(1f - (_movement.x * _movement.x / 2f));

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

        Vector3 _newPosition = MapManager.ClampPositionInRadius(transform.position + _deltaMovement);
        if (charController.enabled)
            charController.Move(_newPosition - transform.position);
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

    public override void Damage(int _damages)
    {
        base.Damage(_damages);
        playerHitFeedback.PlayFeedback();
    }
}
