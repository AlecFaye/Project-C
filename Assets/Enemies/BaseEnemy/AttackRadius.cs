using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{
    public NavMeshAgent agent;
    public SphereCollider sphereCollider;

    protected List<IDamageable> damageables = new();

    public float damage = 10.0f;
    public float attackDelay = 0.5f;

    public delegate void AttackEvent(IDamageable target);
    public AttackEvent OnAttack;

    protected Coroutine attackCoroutine;

    protected IDamageable closestDamageable = null;

    protected virtual void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageables.Add(damageable);

            if (attackCoroutine == null)
            {
                agent.enabled = false;
                attackCoroutine = StartCoroutine(Attack());
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageables.Remove(damageable);

            if (damageables.Count == 0)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    protected virtual IEnumerator Attack()
    {
        WaitForSeconds wait = new(attackDelay);

        closestDamageable = null;
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
            {
                OnAttack?.Invoke(closestDamageable);
            }

            yield return wait;

            closestDamageable = null;
            closestDistance = float.MaxValue;

            damageables.RemoveAll(DisabledDamageables);
        }

        attackCoroutine = null;
    }

    protected bool DisabledDamageables(IDamageable damageable)
    {
        return damageable != null && !damageable.GetTransform().gameObject.activeSelf;
    }

    public void DealDamage()
    {
        if (closestDamageable != null)
        {
            closestDamageable.TakeDamage(damage);
        }
    }
}
