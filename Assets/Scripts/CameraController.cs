using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform followedTransform;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float targetIntensity = 0.5f;

    private void Update()
    {
        if (followedTransform != null)
        {
            Vector3 _wantedPos = followedTransform.position;
            if (cameraTarget != null) { _wantedPos = Vector3.Lerp(_wantedPos, cameraTarget.position, targetIntensity); }
            _wantedPos.y = transform.position.y; //Block camera on Y axis
            transform.position = Vector3.Lerp(transform.position, _wantedPos, Time.deltaTime * moveSpeed);
        }
    }
}
