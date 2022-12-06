using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    public Animator _animator;
    public Transform _damagePopupSpawn;

    // Triggers when Collision Detection script detectst the enemy 
    public void Hit(int damageValue) { //, Weapon damageType) { 
        //Debug.Log("Damage Value: " + damageValue + " == Damage Type: " + damageType);
        _animator.SetTrigger("Hit"); // Sets the trigger "Hit" in the animation controller so that it plays the hit animation
        DamagePopup.Create(_damagePopupSpawn.position, 10); // Creates the popup using the paramaters for (Transform position, int DamageValue)
    }
}
