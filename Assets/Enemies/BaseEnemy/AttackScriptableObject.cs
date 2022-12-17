using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfiguration", menuName = "ScriptableObject/AttackConfiguration")]
public class AttackScriptableObject : ScriptableObject
{
    [Header("Base Attack Configurations")]
    public float damage = 5.0f;
    public float attackDelay = 1.5f;
    public float attackRadius = 1.5f;
    public LayerMask lineOfSightLayers;

    // Flying configurations
    [Header("Flying Configurations")]
    public bool isFlying = false;

    // Ranged configurations
    [Header("Ranged Configurations")]
    public bool isRanged = false;
    public Projectile projectilePrefab;
    public Vector3 projectileSpawnOffset = new(0, 1, 0);

    public void SetupEnemy(Enemy enemy)
    {
        (enemy.attackRadius.sphereCollider == null ? enemy.attackRadius.GetComponent<SphereCollider>() : enemy.attackRadius.sphereCollider).radius = attackRadius;
        enemy.attackRadius.attackDelay = attackDelay;
        enemy.attackRadius.damage = damage;

        if (isRanged)
        {
            RangedAttackRadius rangedAttackRadius = enemy.attackRadius.GetComponent<RangedAttackRadius>();

            rangedAttackRadius.projectilePrefab = projectilePrefab;
            rangedAttackRadius.projectileSpawnOffset = projectileSpawnOffset;
            rangedAttackRadius.mask = lineOfSightLayers;

            rangedAttackRadius.CreateBulletPool();
        }
    }
}
