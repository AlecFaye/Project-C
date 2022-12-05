using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController weaponController;

    

    //checks if any Box Collider has entered this objects == Other refers to the other object being collided with
    private void OnTriggerEnter(Collider other) {
        // Checks if it collided with an enemy ====== Checks if the player should be attacking rn (done in weapon controller) ====== Checks if the enemy was already hit by this attack
        if (other.tag == "Enemy" && weaponController.GetComponent<WeaponController>().IsAttacking && !weaponController.enemiesHitList.Contains(other)) {
            other.GetComponent<CollisionDetected>().Hit(); // Runs the funtion "Hit" in the other objects CollisionDetected script
            weaponController.enemiesHitList.Add(other); // Adds current enemy to enemiesHitList to keep track of
            
            //Debug.Log("Enemy list after being added: " + weaponController.enemiesHitList);
        }
    }

}
