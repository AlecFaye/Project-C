using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    // Triggers when CollisionDetection script detects the enemy 
    public void Hit(float damageDealt, Weapon.WeaponType weaponType) {
        Debug.Log("Damage Value: " + damageDealt + " | Weapon Type: " + weaponType);

        if (enemy)
            enemy.TakeDamage(damageDealt, weaponType);

        if (this.gameObject.tag == "Enemy") {
            Debug.Log(damageDealt);
        }

    }
}

