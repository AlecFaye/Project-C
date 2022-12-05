using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using WeaponController;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController wp;

    //checks if any Box Collider has entered this objects == Other refers to the other object being collided with
    private void OnTriggerEnter(Collider other) {
        // Checks if it collided with an enemy then checks if the player should be attacking rn (done in weapon controller) -> then runs the funtion "Hit" in the other objects CollisionDetected script
        if (other.tag == "Enemy" && wp.GetComponent<WeaponController>().IsAttacking) other.GetComponent<CollisionDetected>().Hit();
    }

}
