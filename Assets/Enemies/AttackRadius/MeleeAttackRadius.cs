using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
public class MeleeAttackRadius : AttackRadius
{
    public SphereCollider sphereCollider;

    private void Awake()
    {
        TryGetComponent(out sphereCollider);
    }

    public override IEnumerator Attack()
    {
        WaitForSeconds wait = new(attackDelay);

        while (damageables.Count > 0)
        {
            IDamageable closestDamageable = ChooseTarget();

            if (closestDamageable != null)
                OnAttack?.Invoke(closestDamageable);

            yield return wait;

            damageables.RemoveAll(DisabledDamageables);
        }
        attackCoroutine = null;
    }

    public IDamageable ChooseTarget()
    {
        IDamageable closestTarget = null;
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = new();

        for (int index = 0; index < damageables.Count; index++)
        {
            IDamageable currentTarget = damageables[index];

            if (currentTarget == null)
                continue;

            if (NavMesh.CalculatePath(transform.position, currentTarget.GetTransform().position, agent.areaMask, path))
            {
                float distance = Vector3.Distance(transform.position, path.corners[0]);

                for (int cornerIndex = 1; cornerIndex < path.corners.Length; cornerIndex++)
                    distance += Vector3.Distance(path.corners[cornerIndex - 1], path.corners[cornerIndex]);

                if (distance < closestTargetDistance)
                {
                    closestTargetDistance = distance;
                    closestTarget = currentTarget;
                }
            }
        }

        return closestTarget;
    }

    public void DealDamage()
    {
        foreach (IDamageable damageable in damageables)
        {
            damageable.TakeDamage(damage);
        }
    }
}
