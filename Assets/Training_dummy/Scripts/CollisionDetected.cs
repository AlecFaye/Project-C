using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    public Weapon pfWeapon;

    public Animator _animator;
    public Transform _damagePopupSpawn;

    // Triggers when Collision Detection script detectst the enemy 
    public void Hit(float damageValue, Weapon.WeaponType weaponType) {
        Debug.Log("Damage Value: " + damageValue + " | Weapon Type: " + weaponType);
        _animator.SetTrigger("Hit"); // Sets the trigger "Hit" in the animation controller so that it plays the hit animation
        DamagePopup.Create(_damagePopupSpawn.position, damageValue); // Creates the popup using the paramaters for (Transform position, int DamageValue)
    }
}
