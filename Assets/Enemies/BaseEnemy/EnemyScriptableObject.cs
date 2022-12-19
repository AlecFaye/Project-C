using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ArmourType { None, VeryLow, Low, Medium, High, VeryHigh }

[CreateAssetMenu(fileName = "EnemyConfiguration", menuName = "ScriptableObject/EnemyConfiguration")]
public class EnemyScriptableObject : ScriptableObject
{
    public Enemy enemyPrefab;
    public AttackScriptableObject attackConfiguration;
    public ParticleSystem particleSystem;

    [Header("Enemy Base Stats")]
    public float health = 100.0f;
    public ArmourType armourType = ArmourType.Medium;
    public List<Weapon.WeaponType> weaknessTypes = new() { Weapon.WeaponType.Axe, Weapon.WeaponType.Bow };
    public float weaknessDamageMultiplier = 1.5f;

    [Header("Enemy Movement Stats")]
    public EnemyState defaultState;
    public float idleLocationRadius = 6.0f;
    public float idleMovespeedMultiplier = 0.5f;
    public float unawareLineOfSightRadius = 6.0f;
    public float awareLineOfSightRadius = 10.0f;
    public float fieldOfView = 90.0f;
    [Range(2, 10)] public int numOfWaypoints = 4;

    [Header("NavMeshAgent Configurations")]
    public float aiUpdateInterval = 0.1f;

    public float acceleration = 8.0f;
    public float angularSpeed = 120.0f;

    public int areaMask = -1;
    public int avoidancePriority = 50;

    public float baseOffset = 0.0f;
    public float height = 2.0f;

    public ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

    public float radius = 0.5f;
    public float speed = 3.0f;
    public float stoppingDistance = 0.5f;

    public void SetupEnemy(Enemy enemy)
    {
        enemy.currentHealth = health;
        enemy.maxHealth = health;
        enemy.armourType = armourType;
        enemy.weaknessTypes = weaknessTypes;
        enemy.weaknessDamageMultiplier = weaknessDamageMultiplier;
        enemy.damage = attackConfiguration.damage;
        enemy.attackDelay = attackConfiguration.attackDelay;

        enemy.agent.acceleration = acceleration;
        enemy.agent.angularSpeed = angularSpeed;
        enemy.agent.areaMask = areaMask;
        enemy.agent.avoidancePriority = avoidancePriority;
        enemy.agent.baseOffset = baseOffset;
        enemy.agent.obstacleAvoidanceType = obstacleAvoidanceType;
        enemy.agent.radius = radius;
        enemy.agent.speed = speed;
        enemy.agent.stoppingDistance = stoppingDistance;

        enemy.movement.updateRate = aiUpdateInterval;
        enemy.movement.defaultState = defaultState;
        enemy.movement.idleLocationRadius = idleLocationRadius;
        enemy.movement.idleMoveSpeedMultiplier = idleMovespeedMultiplier;
        enemy.movement.waypoints = new Vector3[numOfWaypoints];
        enemy.movement.lineOfSightChecker.fieldOfView = fieldOfView;
        enemy.movement.lineOfSightChecker.sphereCollider.radius = unawareLineOfSightRadius;
        enemy.movement.lineOfSightChecker.unawareLineOfSightRadius = unawareLineOfSightRadius;
        enemy.movement.lineOfSightChecker.awareLineOfSightRadius = awareLineOfSightRadius;
        enemy.movement.lineOfSightChecker.lineOfSightLayers = attackConfiguration.lineOfSightLayers;

        attackConfiguration.SetupEnemy(enemy);
    }
}
