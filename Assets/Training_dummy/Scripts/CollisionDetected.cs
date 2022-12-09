using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    [SerializeField] private Weapon pfWeapon;
    //[SerializeField] private Weapon.WeaponType weaponType = Weapon.WeaponType;
    

    [SerializeField] private Animator animator;
    [SerializeField] private Enemy enemy;

    // Triggers when CollisionDetection script detects the enemy 
    public void Hit(float damageDealt, Weapon.WeaponType weaponType) {
        Debug.Log("Damage Value: " + damageDealt + " | Weapon Type: " + weaponType);
<<<<<<< HEAD

        if (enemy) 
=======
        if (enemy)
>>>>>>> main
            enemy.TakeDamage(damageDealt, weaponType);

        if (animator)
        {
            if (CompareTag("TrainingDummy"))
                animator.SetTrigger("Hit");
        }
    }
}
