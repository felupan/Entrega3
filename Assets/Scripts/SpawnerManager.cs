using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Enemies;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float minSpawnDistance;

    public static event Action OnEnemyDeath;
    
    private int enemiesPerRound = 10;
    private int currentEnemyCount;
    private float spawnTimer;

    private bool isSpawning;


    private void Start()
    {
        SpawnRound();
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnRound();
            spawnTimer = 0;
        }
        else if (currentEnemyCount <= 5 && !isSpawning)
        {
            SpawnRound();
            spawnTimer = 0;
        }
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints.Where(sp => Vector3.Distance(sp.position, player.position) >= minSpawnDistance)
            .OrderByDescending(sp => Vector3.Distance(sp.position, player.position))
            .First();
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        EnemyMain main = enemy.GetComponent<EnemyMain>();
        main.MovementSystem.SetTarget(player);
        main.Health.OnDeath += () =>
        {
            OnEnemyDeath?.Invoke();
            currentEnemyCount--;
        };
        currentEnemyCount++;
    }

    private void SpawnRound()
    {
        StartCoroutine(SpawnRoundCoroutine());
    }
    
    private IEnumerator SpawnRoundCoroutine()
    {
        isSpawning = true;
        for (int i = 0; i < enemiesPerRound; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f);
        }
        enemiesPerRound += 5;
        isSpawning = false;
    }
}
