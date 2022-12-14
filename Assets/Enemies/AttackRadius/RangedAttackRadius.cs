using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RangedAttackRadius : AttackRadius
{
    public SphereCollider sphereCollider;
    public Projectile projectilePrefab;
    public Vector3 projectileSpawnOffset = new(0, 1, 0);
    public LayerMask mask;

    private ObjectPool projectilePool;
    [SerializeField] private float sphereCastRadius = 0.1f;
    private RaycastHit hit;
    private IDamageable targetDamageable;
    private Projectile projectile;

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (attackCoroutine == null)
        {
            agent.enabled = true;
        }
    }

    public void CreateProjectilePool()
    {
        if (projectilePool == null)
        {
            projectilePool = ObjectPool.CreateInstance(projectilePrefab, Mathf.CeilToInt((1 / attackDelay) * projectilePrefab.autoDestroyTime));
        }
    }

    public override IEnumerator Attack()
    {
        WaitForSeconds waitAttackCooldown = new(attackDelay);
       
        while (damageables.Count > 0)
        {
            for (int index = 0; index < damageables.Count; index++)
            {
                targetDamageable = damageables[index];

                if (HasLineOfSightTo(targetDamageable.GetTransform()))
                {
                    OnAttack?.Invoke(damageables[index]);
                    agent.enabled = false;
                    break;
                }
            }

            yield return waitAttackCooldown;

            if (targetDamageable == null || !HasLineOfSightTo(targetDamageable.GetTransform()))
            {
                agent.enabled = true;
            }

            damageables.RemoveAll(DisabledDamageables);
        }

        agent.enabled = true;
        attackCoroutine = null;
    }

    private bool HasLineOfSightTo(Transform target)
    {
        if (!Physics.SphereCast(
                transform.position + projectileSpawnOffset, 
                sphereCastRadius, 
                ((target.position + projectileSpawnOffset) - (transform.position + projectileSpawnOffset)).normalized, 
                out hit, 
                sphereCollider.radius, 
                mask))
            return false;

        if (!hit.collider.TryGetComponent(out IDamageable damageable))
            return false;

        return damageable.GetTransform() == target;
    }

    public void ReleaseProjectile()
    {
        if (targetDamageable != null)
        {
            PoolableObject poolableObject = projectilePool.GetObject();

            if (poolableObject)
            {
                projectile = poolableObject.GetComponent<Projectile>();

                projectile.damager = damager;
                projectile.damage = damage;
                projectile.transform.position = transform.position + projectileSpawnOffset;
                projectile.transform.LookAt(targetDamageable.GetTransform().position + projectileSpawnOffset);
                projectile.rb.AddForce(projectile.transform.forward * projectilePrefab.moveSpeed, ForceMode.VelocityChange);
            }
        }
        else
        {
            agent.enabled = true;
        }
    }
}
