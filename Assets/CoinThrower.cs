using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CoinThrower : MonoBehaviour
{
    [SerializeField] private MinMaxCurve throwerAmountByHappiness = new MinMaxCurve(1, new AnimationCurve(), new AnimationCurve());
    [SerializeField] private MinMaxCurve throwDelayMultiplierByHappiness = new MinMaxCurve(1, new AnimationCurve(), new AnimationCurve());
    [SerializeField] private float defaultThrowDelay = 10f;

    [SerializeField] private GameObject coinPrefab;

    [SerializeField] private float coinThrowDuration = 1.2f;
    [SerializeField] private float coinThrowHeight = 3f;
    [SerializeField] private Ease coinThrowEase = Ease.OutSine;

    [SerializeField] private float coinMaxRadiusAroundPlayer = 5f; //Precision

    public static CoinThrower instance;
    private float delayBetweenNextThrow;
    private void Awake()
    {
        if (instance != null) { Destroy(instance); }
        instance = this;
        delayBetweenNextThrow = defaultThrowDelay;
    }

    private void Update()
    {
        float _currentHappiness = FameController.GetHappinessNormalized();
        if (_currentHappiness > 0 && !ArenaController.IsFrozen())
        {
            delayBetweenNextThrow -= Time.deltaTime * throwDelayMultiplierByHappiness.Evaluate(_currentHappiness, Random.value);
        }
        if (delayBetweenNextThrow <= 0)
        {
            ThrowCoins();
            delayBetweenNextThrow = defaultThrowDelay;
        }
    }

    private void ThrowCoins()
    {
        float _currentHappiness = FameController.GetHappinessNormalized();
        int _coinAmount = (Mathf.RoundToInt(throwerAmountByHappiness.Evaluate(_currentHappiness, Random.value)));

        List<Spectator> _happySpectators = CrowdManager.instance.GetHappySpectators();

        int _coinThrowed = 0;
        for (int i = 0; i < _happySpectators.Count; i++)
        {
            if (_coinThrowed < _coinAmount)
            {
                ThrowCoin(_happySpectators[i]);
                _coinThrowed++;
            }
            else
            {
                //All coins have been launched, exiting loop
                break;
            }
        }
    }

    private void ThrowCoin(Spectator _spectator)
    {
        GameObject _coin = Instantiate(coinPrefab);
        _coin.transform.position = _spectator.transform.position;
        Vector3 _coinDestination = GetPointNearPlayer(coinMaxRadiusAroundPlayer);
        _coinDestination = MapManager.ClampPositionInRadius(_coinDestination);

        _coin.transform.DOJump(_coinDestination, coinThrowHeight, 1, coinThrowDuration).SetEase(coinThrowEase);
    }

    private Vector3 GetPointNearPlayer(float _maxRadius)
    {
        Vector3 _playerPosition = PlayerController.instance.transform.position;
        _playerPosition += new Vector3(Random.insideUnitCircle.x * Random.Range(0, _maxRadius), 0, Random.insideUnitCircle.y * Random.Range(0, _maxRadius));
        return _playerPosition;
    }
}
