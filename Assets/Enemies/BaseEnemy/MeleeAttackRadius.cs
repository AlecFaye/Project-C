using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        IDamageable closestDamageable = null;
        float closestDistance = float.MaxValue;

        while (damageables.Count > 0)
        {
            for (int index = 0; index < damageables.Count; index++)
            {
                Transform damageableTF = damageables[index].GetTransform();
                float distance = Vector3.Distance(transform.position, damageableTF.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDamageable = damageables[index];
                }
            }

            if (closestDamageable != null)
                OnAttack?.Invoke(closestDamageable);

            yield return wait;

            closestDamageable = null;
            closestDistance = float.MaxValue;

            damageables.RemoveAll(DisabledDamageables);
        }

        attackCoroutine = null;
    }

    public void DealDamage()
    {
        foreach (IDamageable damageable in damageables)
        {
            damageable.TakeDamage(damage);
        }
    }
}
