using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public int numberOfEnemiesToSpawn = 3;
    public float spawnDelay = 1.0f;
    public List<EnemyScriptableObject> enemies = new();
    public SpawnMethod enemySpawnMethod = SpawnMethod.RoundRobin;

    private NavMeshTriangulation triangulation;
    private Dictionary<int, ObjectPool> enemyObjectPools = new();

    private void Awake()
    {
        for (int index = 0; index < enemies.Count; index++)
        {
            enemyObjectPools.Add(index, ObjectPool.CreateInstance(enemies[index].enemyPrefab, numberOfEnemiesToSpawn));
        }
    }

    private void Start()
    {
        triangulation = NavMesh.CalculateTriangulation();
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds wait = new(spawnDelay);

        int spawnedEnemies = 0;

        while (spawnedEnemies < numberOfEnemiesToSpawn)
        {
            if (enemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(spawnedEnemies);
            }
            else if (enemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }
            spawnedEnemies++;

            yield return wait;
        }
    }
    
    private void SpawnRoundRobinEnemy(int spawnedEnemies)
    {
        int spawnIndex = spawnedEnemies % enemies.Count;

        DoSpawnEnemy(spawnIndex);
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, enemies.Count));
    }

    private void DoSpawnEnemy(int spawnIndex)
    {
        PoolableObject poolableObject = enemyObjectPools[spawnIndex].GetObject();

        if (poolableObject)
        {
            Enemy enemy = poolableObject.GetComponent<Enemy>();
            enemies[spawnIndex].SetupEnemy(enemy);

            int vertexIndex = Random.Range(0, triangulation.vertices.Length);

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

    public enum SpawnMethod
    {
        RoundRobin,
        Random
    }
}
