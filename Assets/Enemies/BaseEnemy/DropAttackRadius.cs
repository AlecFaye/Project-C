using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class DropAttackRadius : AttackRadius
{
    public CapsuleCollider capsuleCollider;
    public Projectile projectilePrefab;
    public Vector3 projectileSpawnOffset = new(0, 1, 0);
    public LayerMask mask;

    private ObjectPool projectilePool;
    private RaycastHit hit;
    private IDamageable targetDamageable;
    private Projectile projectile;

    [SerializeField] private GameObject fallingObjectIndicator;
    [SerializeField] private LayerMask indicatorMask;

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
        if (!Physics.Raycast(
                transform.position + projectileSpawnOffset,
                ((target.position) - (transform.position)).normalized,
                out hit,
                20.0f,
                mask))
            return false;

        if (!hit.collider.TryGetComponent(out IDamageable damageable))
            return false;

        return damageable.GetTransform() == target;
    }

    public void DropProjectile()
    {
        if (targetDamageable != null)
        {
            DisplayDangerIndicator();

            PoolableObject poolableObject = projectilePool.GetObject();

            if (poolableObject)
            {
                projectile = poolableObject.GetComponent<Projectile>();

                projectile.damage = damage;
                projectile.transform.position = transform.position + projectileSpawnOffset;
                projectile.rb.AddForce(Vector3.down * projectilePrefab.moveSpeed, ForceMode.VelocityChange);
            }
        }
        else
        {
            agent.enabled = true;
        }
    }

    private void DisplayDangerIndicator()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastHit, 20.0f, indicatorMask))
        {
            Instantiate(fallingObjectIndicator, raycastHit.point, Quaternion.identity);
        }
    }
}
