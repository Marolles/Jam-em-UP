using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 7f;

    [SerializeField] private float rotationSpeed = 1f;

    private float currentSpeed = 0f;

    private CharacterController charController;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Adapt the square mapping into a circular mapping (prevent diagonal faster movement, but keep joystick nuance)
        _movement.x *= Mathf.Sqrt(1f - (_movement.z * _movement.z / 2f));
        _movement.z *= Mathf.Sqrt(1f - (_movement.x * _movement.x / 2f));

        if (_movement.magnitude > 0) //Handle acceleration if player is moving
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * acceleration);

            //Also apply rotation
            transform.forward = Vector3.Lerp(transform.forward, _movement, Time.deltaTime * rotationSpeed);
        } else
        { //Else, handle deceleration if player is not moving
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * deceleration);
        }

        //Apply movement to characterController
        charController.Move(_movement * Time.deltaTime * currentSpeed);

    }
}
