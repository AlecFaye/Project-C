using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public int numberOfEnemiesToSpawn = 3;
    public float spawnDelay = 1.0f;
    public List<Enemy> enemyPrefabs = new();
    public SpawnMethod enemySpawnMethod = SpawnMethod.RoundRobin;

    private NavMeshTriangulation triangulation;
    private Dictionary<int, ObjectPool> enemyObjectPools = new();

    private void Awake()
    {
        for (int index = 0; index < enemyPrefabs.Count; index++)
        {
            enemyObjectPools.Add(index, ObjectPool.CreateInstance(enemyPrefabs[index], numberOfEnemiesToSpawn));
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
        int spawnIndex = spawnedEnemies % enemyPrefabs.Count;

        DoSpawnEnemy(spawnIndex);
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, enemyPrefabs.Count));
    }

    private void DoSpawnEnemy(int spawnIndex)
    {
        PoolableObject poolableObject = enemyObjectPools[spawnIndex].GetObject();

        if (poolableObject)
        {
            Enemy enemy = poolableObject.GetComponent<Enemy>();

            int vertexIndex = Random.Range(0, triangulation.vertices.Length);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out hit, 2.0f, 1))
            {
                enemy.agent.Warp(hit.position);
                enemy.agent.enabled = true;
            }
            else
            {
                Debug.LogError($"Unable to palce NavMeshAgent on NavMesh. Tried to use {triangulation.vertices[vertexIndex]}");
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
