using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Spawner[] spawners;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float delayBetweenSpawns = 5f;
    [SerializeField] private int maxSimultaneousEnemies = 10;


    public static List<EnemyController> currentEnemies = new List<EnemyController>();

    private void Awake()
    {
        StartWave();
    }

    public void StartWave()
    {
        SpawnNextEnemy();
    }

    private void SpawnNextEnemy()
    {
        if (currentEnemies.Count < maxSimultaneousEnemies) 
        {
            Spawner _randomSpawner = GetRandomSpawner();
            _randomSpawner.SpawnEnemy(enemyPrefab);
        }
        Invoke("SpawnNextEnemy", delayBetweenSpawns);
    }

    private Spawner GetRandomSpawner()
    {
        return (spawners[Random.Range(0, spawners.Length - 1)]);
    }
}
