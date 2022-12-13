using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    // Triggers when CollisionDetection script detects the enemy 
    public void Hit(float damageValue, Weapon.WeaponType weaponType, Vector3 hitPosition) {
        if (enemy) {
            enemy.TakeDamage(damageValue, weaponType);
        }
        DamagePopup.Create(hitPosition, damageValue);
    }
}

