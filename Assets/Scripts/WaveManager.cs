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
        currentEnemies.Clear();
    }
    private void Start()
    {
        StartWave();
    }

    public void StartWave()
    {
        SpawnNextEnemy();
        Debug.Log("Wave started");
    }

    private void SpawnNextEnemy()
    {
        if (currentEnemies.Count < maxSimultaneousEnemies) 
        {
            Spawner _randomSpawner = GetRandomSpawner();
            Debug.Log("Enemy count is correct, spawning new enemy");
            _randomSpawner.SpawnEnemy(enemyPrefab);
        } else
        {
            Debug.Log("Too much enemies, not spawning a new ");
        }
        Invoke("SpawnNextEnemy", delayBetweenSpawns);
    }

    private Spawner GetRandomSpawner()
    {
        return (spawners[Random.Range(0, spawners.Length - 1)]);
    }
}
