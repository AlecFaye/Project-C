using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ArmourType { VeryLow, Low, Medium, High, VeryHigh }

[CreateAssetMenu(fileName = "EnemyConfiguration", menuName = "ScriptableObject/EnemyConfiguration")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Enemy Stats")]
    public float health = 100.0f;
    public ArmourType armourType = ArmourType.Medium;
    public List<Weapon.WeaponType> weaknessTypes = new() { Weapon.WeaponType.Axe, Weapon.WeaponType.Bow };
    public float weaknessDamageMultiplier = 1.5f;

    public bool isRanged = false;
    public float attackDamage = 10.0f;
    public float attackDelay = 5.0f;
    public float attackRadius = 1.5f;

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
}
