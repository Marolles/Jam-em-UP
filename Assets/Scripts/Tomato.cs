using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomato : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    private Vector3 randomDirection;

    private void Start()
    {
        randomDirection = Random.onUnitSphere;
    }
    void Update()
    {
        transform.Rotate(randomDirection, rotationSpeed * Time.deltaTime);
    }
}
