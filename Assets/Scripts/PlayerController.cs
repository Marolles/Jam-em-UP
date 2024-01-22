using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : Hitable
{
    public static PlayerController instance;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 7f;

    [SerializeField] private float rotationSpeed = 5f;

    private float currentSpeed = 0f;

    private CharacterController charController;

    private Plane groundPlane;
    private Vector3 wantedLookedPos;

    protected override void Awake()
    {
        base.Awake();
        if (instance != null) { Debug.LogError("Can't have 2 players at the same time, destroying first one."); Destroy(instance.gameObject); }
        instance = this;

        charController = GetComponent<CharacterController>();

        groundPlane = new Plane(Vector3.up, Vector3.zero);//Generate a virtual plane at Y = 0 to check for mouse raycasts
    }

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

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<AttackController>().Attack();
        }
    }

    private void HandleMovement()
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
        Vector3 _deltaMovement = _movement * Time.deltaTime * currentSpeed;

        Vector3 _newPosition = MapManager.ClampPositionInRadius(transform.position + _deltaMovement);
        charController.Move(_newPosition - transform.position);
    }

    private void HandleRotation()
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
}
