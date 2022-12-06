using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Transform damagePopupSpawn;

    // Triggers when CollisionDetection script detects the enemy 
    public void Hit(float damageDealt = 10.0f, int damageType = 0) {
        if (enemy) 
            enemy.TakeDamage(damageDealt, damageType);

        if (animator)
            animator.SetTrigger("Hit");

        if (damagePopupSpawn)
            DamagePopup.Create(damagePopupSpawn.position, 10);
    }
}
