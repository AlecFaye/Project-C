using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Melee, Ranged, Fleeing, Dropping }

[CreateAssetMenu(fileName = "AttackConfiguration", menuName = "ScriptableObject/AttackConfiguration")]
public class AttackScriptableObject : ScriptableObject
{
    [Header("Base Attack Configurations")]
    public float damage = 5.0f;
    public float attackDelay = 1.5f;
    public float attackRadius = 1.5f;
    public LayerMask lineOfSightLayers;

    public AttackType attackType = AttackType.Melee;

    [Header("Ranged Attack Configurations")]
    public Projectile projectilePrefab;
    public Vector3 projectileSpawnOffset = new(0, 1, 0);

    public void SetupEnemy(Enemy enemy)
    {
        enemy.attackRadius.attackDelay = attackDelay;
        enemy.attackRadius.damage = damage;

        switch (attackType)
        {
            case AttackType.Melee: case AttackType.Fleeing:
                break;
            case AttackType.Ranged:
                RangedAttackRadius rangedAttackRadius = enemy.attackRadius.GetComponent<RangedAttackRadius>();

                (rangedAttackRadius.sphereCollider == null ? rangedAttackRadius.GetComponent<SphereCollider>() : rangedAttackRadius.sphereCollider).radius = attackRadius;

                rangedAttackRadius.projectilePrefab = projectilePrefab;
                rangedAttackRadius.projectileSpawnOffset = projectileSpawnOffset;
                rangedAttackRadius.mask = lineOfSightLayers;

                rangedAttackRadius.CreateProjectilePool();

                break;
            case AttackType.Dropping:
                DropAttackRadius dropAttackRadius = enemy.attackRadius.GetComponent<DropAttackRadius>();

                dropAttackRadius.projectilePrefab = projectilePrefab;
                dropAttackRadius.projectileSpawnOffset = projectileSpawnOffset;
                dropAttackRadius.mask = lineOfSightLayers;

                dropAttackRadius.CreateProjectilePool();

                break;
        }
    }
}
