using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeFunction : MonoBehaviour
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

    private void OnTriggerEnter(Collider other) 
    {
        // Checks if it collided with an enemy ====== Checks if the player should be attacking rn(done in weapon controller) ====== Checks if the enemy was already hit by this attack
        if (weaponController.GetComponent<WeaponController>().IsAttacking && !weaponController.enemiesHitList.Contains(other))
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(weapon.damageValue, weapon.weaponType);
                weaponController.enemiesHitList.Add(other);
            }
        }
    }
}
