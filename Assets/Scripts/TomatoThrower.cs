using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class TomatoThrower : MonoBehaviour
{
    [SerializeField] private MinMaxCurve throwerAmountByAngriness = new MinMaxCurve(1, new AnimationCurve(), new AnimationCurve());
    [SerializeField] private MinMaxCurve throwDelayMultiplierByAngriness = new MinMaxCurve(1, new AnimationCurve(), new AnimationCurve());
    [SerializeField] private float defaultThrowDelay = 10f;

    [SerializeField] private GameObject tomatoPrefab;
    [SerializeField] private GameObject tomatoImpactPrevisualizer;
    [SerializeField] private GameObject tomatoImpactFX;

    [SerializeField] private float tomatoThrowDuration = 1.2f;
    [SerializeField] private float tomatoThrowHeight = 3f;
    [SerializeField] private Ease tomatoThrowEase = Ease.OutSine;
    [SerializeField] private Ease tomatoImpactPrevisualizerEase = Ease.OutSine;

    [SerializeField] private float tomatoMaxRadiusAroundPlayer = 5f; //Precision
    [SerializeField] private int tomatoDamages = 1;
    [SerializeField] private float tomatoImpactRadius = 1;

    public static TomatoThrower instance;
    private float delayBetweenNextThrow;
    private void Awake()
    {
        if (instance != null) { Destroy(instance); }
        instance = this;
        delayBetweenNextThrow = defaultThrowDelay;
    }

    private void Update()
    {
        float _currentAngriness = FameController.GetAngrinessNormalized();
        if (_currentAngriness > 0 && !ArenaController.IsFrozen())
        {
            delayBetweenNextThrow -= Time.deltaTime * throwDelayMultiplierByAngriness.Evaluate(_currentAngriness, Random.value);
        }
        if (delayBetweenNextThrow <= 0)
        {
            ThrowTomatoes();
            delayBetweenNextThrow = defaultThrowDelay;
        }
    }

    private void ThrowTomatoes()
    {
        float _currentAngriness = FameController.GetAngrinessNormalized();
        int _tomatoAmount = (Mathf.RoundToInt(throwerAmountByAngriness.Evaluate(_currentAngriness, Random.value)));

        List<Spectator> _angrySpectator = CrowdManager.instance.GetAngrySpectators();

        int _tomatoThrowed = 0;
        for (int i = 0; i < _angrySpectator.Count; i++)
        {
            if (_tomatoThrowed < _tomatoAmount)
            {
                ThrowTomato(_angrySpectator[i]);
                _tomatoThrowed++;
            } else
            {
                //All tomatoes have been launched, exiting loop
                break;
            }
        }
    }

    private void ThrowTomato(Spectator _spectator)
    {
        GameObject _tomato = Instantiate(tomatoPrefab);
        _tomato.transform.position = _spectator.transform.position;
        Vector3 _tomatoDestination = GetPointNearPlayer(tomatoMaxRadiusAroundPlayer);

        GameObject _impactPrevisualizer = Instantiate(tomatoImpactPrevisualizer);
        _impactPrevisualizer.transform.position = _tomatoDestination;
        _impactPrevisualizer.transform.DOScale(1f, tomatoThrowDuration / 2f).SetEase(tomatoImpactPrevisualizerEase);

        _tomato.transform.DOJump(_tomatoDestination, tomatoThrowHeight, 1, tomatoThrowDuration).SetEase(tomatoThrowEase).OnComplete(() => TomatoImpact(_tomato, _impactPrevisualizer));
    }

    private void TomatoImpact(GameObject _tomato, GameObject _visualizer)
    {
        //Spawn FX
        GameObject _tomatoImpactFX = Instantiate(tomatoImpactFX);
        _tomatoImpactFX.transform.position = _tomato.transform.position;

        //Remove tomato
        Destroy(_visualizer.gameObject);
        Destroy(_tomato.gameObject);

        //Damage player
        foreach (Collider _c in Physics.OverlapSphere(_tomato.transform.position, tomatoImpactRadius))
        {
            if (_c.TryGetComponent(out PlayerController _pc))
            {
                _pc.Damage(tomatoDamages, DamageType.Tomato, _tomato.transform.position);
            }
        }
    }

    private Vector3 GetPointNearPlayer(float _maxRadius)
    {
        Vector3 _playerPosition = PlayerController.instance.transform.position;
        _playerPosition += new Vector3(Random.insideUnitCircle.x * Random.Range(0, _maxRadius), 0, Random.insideUnitCircle.y * Random.Range(0, _maxRadius));
        return _playerPosition;
    }
}
