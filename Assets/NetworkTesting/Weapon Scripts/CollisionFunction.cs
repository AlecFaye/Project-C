using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionFunction : MonoBehaviour
{

    public Weapon weapon;
    
    public WeaponController weaponController;

    private void Start() // Used to setup the parent of the fucntion
    {
        if (weaponController == null)
            weaponController = this.transform.parent.GetComponent<WeaponController>();
        else {
            Debug.Log("No Weapon Set");
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Collided obect's name: " + other.name);

        // Checks if it collided with an enemy ====== Checks if the player should be attacking rn (done in weapon controller) ====== Checks if the enemy was already hit by this attack
        if (other.CompareTag("Enemy") && weaponController.GetComponent<WeaponController>().IsAttacking && !weaponController.enemiesHitList.Contains(other)) {
            CollisionDetected collisionDetected = other.GetComponent<CollisionDetected>();

            if (collisionDetected) {
                collisionDetected.Hit(weapon.damageValue, weapon.weaponType); // Runs the funtion "Hit" in the other objects CollisionDetected script
                weaponController.enemiesHitList.Add(other); // Adds current enemy to enemiesHitList to keep track of
            }
        }
    }
}
