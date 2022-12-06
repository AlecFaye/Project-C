using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    [SerializeField] private Weapon pfWeapon;
    
    [SerializeField] private Animator animator;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Transform damagePopupSpawn;

    // Triggers when CollisionDetection script detects the enemy 
    public void Hit(float damageDealt = 1.0f, Weapon.WeaponType weaponType) {
        Debug.Log("Damage Value: " + damageValue + " | Weapon Type: " + weaponType);
        if (enemy) 
            enemy.TakeDamage(damageDealt, weaponType);

        if (animator)
            animator.SetTrigger("Hit");

        if (damagePopupSpawn)
            DamagePopup.Create(damagePopupSpawn.position, damageDealt);
    }
}
