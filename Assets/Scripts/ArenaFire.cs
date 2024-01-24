using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _slowFirePS;
    [SerializeField] private ParticleSystem _fastFirePS;
    [SerializeField] private GameObject fireBurstFX;
    [SerializeField] private Transform fireEmissionPosition;

    [SerializeField] private float minFireburstSize = 1f;
    [SerializeField] private float maxFireburstSize = 3f;

    private void Start()
    {
        FameController.instance.onFameChange.AddListener(UpdateFlames);
        _slowFirePS.Play();
        _fastFirePS.Stop();
    }

    private void UpdateFlames(float _famePercentChange, float _totalFamePercent)
    {
        if (_totalFamePercent < 0.5f)
        {
            if (_fastFirePS.isPlaying) _fastFirePS.Stop();
            if (!_slowFirePS.isPlaying) _slowFirePS.Play();
        } else
        {
            if (_slowFirePS.isPlaying) _slowFirePS.Stop();
            if (!_fastFirePS.isPlaying) _fastFirePS.Play();
        }
        if (_famePercentChange > 0)
        {
            //Spawn temorary FX
            GameObject _fireBurst = Instantiate(fireBurstFX, transform);
            _fireBurst.transform.position = fireEmissionPosition.position;
            _fireBurst.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            _fireBurst.transform.localScale = Vector3.Lerp(Vector3.one * minFireburstSize, Vector3.one * maxFireburstSize, _totalFamePercent);
        }
    }
}
