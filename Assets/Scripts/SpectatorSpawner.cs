using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject spectatorPrefab;

    private void Start()
    {
        GameObject _spectatorPrefab = Instantiate(spectatorPrefab, transform);
        _spectatorPrefab.transform.position = transform.position;
    }
}
