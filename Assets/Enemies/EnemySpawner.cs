using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnMethod { Random, RoundRobin }

    [SerializeField] private int numberOfEnemiesToSpawn = 3;
    [SerializeField] private float spawnDelay = 1.0f;
    [SerializeField] private List<EnemyScriptableObject> enemies = new();
    [SerializeField] private SpawnMethod enemySpawnMethod = SpawnMethod.Random;

    private NavMeshTriangulation triangulation;
    private Dictionary<int, ObjectPool> enemyObjectPools = new();
    private Dictionary<int, int> enemyGroupings = new();
    
    private int activeEnemies = 0;

    private void Awake()
    {
        for (int index = 0; index < enemies.Count; index++)
        {
            enemyObjectPools.Add(index, ObjectPool.CreateInstance(enemies[index].enemyPrefab, numberOfEnemiesToSpawn));
            enemyGroupings.Add(index, enemies[index].groupingCount);
        }
    }

    private void Start()
    {
        triangulation = NavMesh.CalculateTriangulation();
        StartCoroutine(SpawnEnemies());
    }

    public void SetMaxEnemiesToSpawn(int spawnCount)
    {
        numberOfEnemiesToSpawn = spawnCount;
    }

    public void DecreaseEnemyCount(int count = 1)
    {
        activeEnemies -= count;
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds waitSpawnDelay = new(spawnDelay);
        WaitForSeconds wait = new(5.0f);

        int currentRoundRobinIndex = 0;

        while(true)
        {
            while (activeEnemies < numberOfEnemiesToSpawn)
            {
                int newlySpawnedEnemies = 0;

                switch (enemySpawnMethod)
                {
                    case SpawnMethod.Random:
                        newlySpawnedEnemies = SpawnRandomEnemy();
                        break;
                    case SpawnMethod.RoundRobin:
                        newlySpawnedEnemies = SpawnRoundRobinEnemy(currentRoundRobinIndex);
                        currentRoundRobinIndex = currentRoundRobinIndex >= enemies.Count - 1 ? 0 : currentRoundRobinIndex + 1;
                        break;
                }

                activeEnemies += newlySpawnedEnemies;

                yield return waitSpawnDelay;
            }
            yield return wait;
        }
    }

    private int SpawnRoundRobinEnemy(int spawnIndex)
    {
        int numberOfEnemiesToSpawn = enemyGroupings[spawnIndex];

        DoSpawnEnemy(spawnIndex, numberOfEnemiesToSpawn);

        return numberOfEnemiesToSpawn;
    }

    private int SpawnRandomEnemy()
    {
        int spawnIndex = Random.Range(0, enemies.Count);
        int numberOfEnemiesToSpawn = enemyGroupings[spawnIndex];

        DoSpawnEnemy(spawnIndex, numberOfEnemiesToSpawn);

        return numberOfEnemiesToSpawn;
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
                enemies[spawnIndex].SetupEnemy(enemy);

                if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out NavMeshHit hit, 2f, -1))
                {
                    enemy.agent.Warp(hit.position);
                    enemy.agent.enabled = true;

                    enemy.movement.triangulation = triangulation;
                    enemy.movement.Spawn();
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
}
