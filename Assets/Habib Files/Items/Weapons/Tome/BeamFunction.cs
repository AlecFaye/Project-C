using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeamFunction : MonoBehaviour
{
    public float damageValue;

    private IDamageable playerStats;
    private List<IDamageable> enemiesHitList = new List<IDamageable>(); // Makes a list to keep track of which enemies were hit

    public IEnumerator Create(IDamageable playerStats, float damage) {
        this.playerStats = playerStats;
        this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true; // Testing stuff
        damageValue = damage;
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (enemiesHitList.Contains(damageable)) return;

        damageable.TakeDamage(playerStats, damageValue, Weapon.WeaponType.Tome);
        enemiesHitList.Add(damageable);
    }
}
