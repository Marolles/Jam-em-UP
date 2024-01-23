using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Spawner[] spawners;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private AnimationCurve delayBetweenSpawnsOverTime;
    [SerializeField] private AnimationCurve maxSimultaneousEnemiesOverTime;


    public static List<EnemyController> currentEnemies = new List<EnemyController>();

    private float currentDuration;
    private bool waveStarted = false;

    private void Awake()
    {
        currentEnemies.Clear();
    }
    private void Start()
    {
        StartWave();
    }

    private void Update()
    {
        if (waveStarted && !ArenaController.IsFrozen())
            currentDuration += Time.deltaTime;
    }

    public void StartWave()
    {
        waveStarted = true;
        SpawnNextEnemy();
        Debug.Log("Wave started");
        currentDuration = 0f;
    }

    private void SpawnNextEnemy()
    {
        int _currentMaxEnemyCount = Mathf.RoundToInt(maxSimultaneousEnemiesOverTime.Evaluate(currentDuration));
        if (currentEnemies.Count < _currentMaxEnemyCount && !ArenaController.IsFrozen()) 
        {
            Spawner _randomSpawner = GetRandomSpawner();
            _randomSpawner.SpawnEnemy(enemyPrefab);
        }
        float _currentDelayBetweenSpawns = delayBetweenSpawnsOverTime.Evaluate(currentDuration);
        Invoke("SpawnNextEnemy", _currentDelayBetweenSpawns);
    }

    private Spawner GetRandomSpawner()
    {
        return (spawners[Random.Range(0, spawners.Length - 1)]);
    }
}
