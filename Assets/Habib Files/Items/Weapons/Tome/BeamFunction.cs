using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeamFunction : MonoBehaviour
{
    public float damageValue;

    private List<IDamageable> enemiesHitList = new List<IDamageable>(); // Makes a list to keep track of which enemies were hit

    public IEnumerator Create(float damage) {
        damageValue = damage;
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (enemiesHitList.Contains(damageable)) return;

        damageable.TakeDamage(damageValue, Weapon.WeaponType.Tome);
        enemiesHitList.Add(damageable);
    }
}
