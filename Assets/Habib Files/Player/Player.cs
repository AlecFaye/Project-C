using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public void TakeDamage(IDamageable damager, float damageTaken, Weapon.WeaponType damageType = Weapon.WeaponType.None) {
        // Debug.Log($"Taking {damageTaken} Damage!");
    }

    public Transform GetTransform() {
        return transform;
    }
}
