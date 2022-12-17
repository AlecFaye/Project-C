using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damageTaken, Weapon.WeaponType damageType = Weapon.WeaponType.None);

    Transform GetTransform();
}
