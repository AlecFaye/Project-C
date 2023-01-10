using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnMethod { Random, RoundRobin }

    [SerializeField] private int numberOfEnemiesToSpawn = 3;
    [SerializeField] private float spawnDelay = 2.5f;
    [SerializeField] private float waveDelay = 5.0f;
    [SerializeField] private List<EnemyScriptableObject> enemies = new();
    [SerializeField] private SpawnMethod enemySpawnMethod = SpawnMethod.Random;
    [SerializeField] private bool continuousSpawning = false;
    [SerializeField] private ScalingScriptableObject scaling;

    [Space]

    [Header("Read at Runtime")]
    [SerializeField] private int wave = 0;
    [SerializeField] private int lastWave = 1;
    [SerializeField] private List<EnemyScriptableObject> scaledEnemies = new();

    [SerializeField] private int enemiesAlive = 0;
    [SerializeField] private int spawnedEnemies = 0;

    private int initialEnemiesToSpawn;
    private float initialSpawnDelay;

    private NavMeshTriangulation triangulation;
    private Dictionary<int, ObjectPool> enemyObjectPools = new();
    private Dictionary<int, int> enemyGroupings = new();

    private void Awake()
    {
        for (int index = 0; index < enemies.Count; index++)
        {
            enemyObjectPools.Add(index, ObjectPool.CreateInstance(enemies[index].enemyPrefab, numberOfEnemiesToSpawn));
            enemyGroupings.Add(index, enemies[index].groupingCount);
        }

        initialEnemiesToSpawn = numberOfEnemiesToSpawn;
        initialSpawnDelay = spawnDelay;
    }

    private void Start()
    {
        triangulation = NavMesh.CalculateTriangulation();

        for (int index = 0; index < enemies.Count; index++)
        {
            scaledEnemies.Add(enemies[index].ScaleUpForLevel(scaling, 0));
        }

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator StartWaveCountdown()
    {
        yield return new WaitForSeconds(waveDelay);

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        if (wave >= lastWave)
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            
            if (levelManager != null)
            {
                levelManager.UnlockNextLevel();
                levelManager.LoadNextLevel();
            }
        }

        wave++;
        spawnedEnemies = 0;
        enemiesAlive = 0;

        ScaleUpEnemies();

        WaitForSeconds waitSpawnDelay = new(spawnDelay);

        int currentRoundRobinIndex = 0;

        while (spawnedEnemies < numberOfEnemiesToSpawn)
        {
            switch (enemySpawnMethod)
            {
                case SpawnMethod.Random:
                    SpawnRandomEnemy();
                    break;
                case SpawnMethod.RoundRobin:
                    SpawnRoundRobinEnemy(currentRoundRobinIndex);
                    currentRoundRobinIndex = currentRoundRobinIndex >= enemies.Count - 1 ? 0 : currentRoundRobinIndex + 1;
                    break;
            }
            yield return waitSpawnDelay;
        }

        if (continuousSpawning)
        {
            ScaleUpSpawns();
            StartCoroutine(StartWaveCountdown());
        }
    }

    private void SpawnRoundRobinEnemy(int spawnIndex)
    {
        int numberOfEnemiesToSpawn = enemyGroupings[spawnIndex];

        DoSpawnEnemy(spawnIndex, numberOfEnemiesToSpawn);
    }

    private void SpawnRandomEnemy()
    {
        int spawnIndex = Random.Range(0, enemies.Count);
        int numberOfEnemiesToSpawn = enemyGroupings[spawnIndex];

        DoSpawnEnemy(spawnIndex, numberOfEnemiesToSpawn);
    }

    private void DoSpawnEnemy(int spawnIndex, int numberOfEnemiesToSpawn)
    {
        int vertexIndex = Random.Range(0, triangulation.vertices.Length);

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            PoolableObject poolableObject = enemyObjectPools[spawnIndex].GetObject();

            if (poolableObject)
            {
                Enemy enemy = poolableObject.GetComponent<Enemy>();
                scaledEnemies[spawnIndex].SetupEnemy(enemy);

                if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out NavMeshHit hit, 2f, -1))
                {
                    enemy.agent.Warp(hit.position);
                    enemy.agent.enabled = true;

                    enemy.movement.triangulation = triangulation;
                    enemy.movement.Spawn();

                    enemy.OnDie += HandleDeathEvent;

                    enemiesAlive++;
                    spawnedEnemies++;
                }
                else
                {
                    Debug.LogError($"Unable to place {enemy.name} on NavMesh. Tried to use {triangulation.vertices[vertexIndex]}");
                }
            }
            else
            {
                Debug.LogError($"Unable to fetch enemy of type {spawnIndex} from object pool. Out of objects.");
            }
        }
    }

    private void ScaleUpEnemies()
    {
        for (int index = 0; index < enemies.Count; index++)
        {
            scaledEnemies[index] = enemies[index].ScaleUpForLevel(scaling, wave);
        }
    }

    private void ScaleUpSpawns()
    {
        numberOfEnemiesToSpawn = Mathf.FloorToInt(initialEnemiesToSpawn * scaling.spawnCountCurve.Evaluate(wave + 1));
        spawnDelay = initialSpawnDelay * scaling.spawnRateCurve.Evaluate(wave + 1);
    }

    private void HandleDeathEvent(Enemy enemy)
    {
        enemiesAlive--;

        if (enemiesAlive == 0 && spawnedEnemies >= numberOfEnemiesToSpawn)
        {
            ScaleUpSpawns();
            StartCoroutine(StartWaveCountdown());
        }
    }
}
