using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamFunction : MonoBehaviour
{
    public float damageValue;

    public List<Collider> enemiesHitList = new List<Collider>(); // Checks if the arrow has hit a target (true == max one target hit, false == max one target not hit yet)

    public IEnumerator Create(float damage)
    {
        damageValue = damage;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (!enemiesHitList.Contains(other)) {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damageValue, Weapon.WeaponType.Tome);
                enemiesHitList.Add(other); // Adds current enemy to enemiesHitList to keep track of
            }
        }
    }
}
