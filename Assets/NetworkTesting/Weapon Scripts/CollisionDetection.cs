using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController wp;

    List<Collider> enemiesHitList = new List<Collider>(); // Makes a list to keep track of which enemies were hit

    //checks if any Box Collider has entered this objects == Other refers to the other object being collided with
    private void OnTriggerEnter(Collider other) {
        // Checks if it collided with an enemy ====== Checks if the player should be attacking rn (done in weapon controller) ====== Checks if the enemy was already hit by this attack
        if (other.tag == "Enemy" && wp.GetComponent<WeaponController>().IsAttacking) {
            other.GetComponent<CollisionDetected>().Hit(); // Runs the funtion "Hit" in the other objects CollisionDetected script
            enemiesHitList.Add(other); // Adds current enemy to enemiesHitList to keep track of
        }
    }

}
