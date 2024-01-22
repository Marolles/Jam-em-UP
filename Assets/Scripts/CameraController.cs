using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform followedTransform;
    [SerializeField] private float moveSpeed = 5f;


    private void Update()
    {
        if (followedTransform != null)
        {
            Vector3 _wantedPos = followedTransform.position;
            _wantedPos.y = transform.position.y; //Block camera on Y axis
            transform.position = Vector3.Lerp(transform.position, _wantedPos, Time.deltaTime * moveSpeed);
        }
    }
}
