using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

abstract public class AttackRadius : MonoBehaviour
{
    public NavMeshAgent agent;

    public float damage = 10.0f;
    public float attackDelay = 0.5f;

    public delegate void AttackEvent(IDamageable target);
    public AttackEvent OnAttack;

    protected List<IDamageable> damageables = new();
    protected Coroutine attackCoroutine;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable damageable))
            return;

        damageables.Add(damageable);

        if (attackCoroutine == null)
        {
            agent.enabled = false;
            attackCoroutine = StartCoroutine(Attack());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable damageable))
            return;

        damageables.Remove(damageable);

        if (damageables.Count <= 0)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    protected bool DisabledDamageables(IDamageable damageable)
    {
        return damageable != null && !damageable.GetTransform().gameObject.activeSelf;
    }

    
    abstract public IEnumerator Attack();
}
