using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(IDamageable damager, float damageTaken, Weapon.WeaponType damageType = Weapon.WeaponType.None);

    Transform GetTransform();
}
