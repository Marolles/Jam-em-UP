using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    [SerializeField] private float mapRadius;

    private void Awake()
    {
        if (instance != null) { Debug.LogError("Map manager already defined, removing first"); Destroy(instance); }
        instance = this;
    }

    public static Vector3 ClampPositionInRadius(Vector3 _position)
    {
        float distanceToCenter = Vector3.Distance(Vector3.zero, _position);

        if (distanceToCenter > instance.mapRadius)
        {
            Vector3 directionToCenter = (_position - Vector3.zero).normalized;
            _position = Vector3.zero + directionToCenter * instance.mapRadius;
        }

        return _position;
    }
}
