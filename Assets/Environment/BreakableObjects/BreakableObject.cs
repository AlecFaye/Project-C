using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(Rigidbody))]
public class BreakableObject : MonoBehaviour, IDamageable
{
    [SerializeField] private float health;
    [SerializeField] private Weapon.WeaponType weakness = Weapon.WeaponType.None;
    [SerializeField] private Transform damagePopupSpawn;

    public delegate void DestroyEvent(BreakableObject breakableObject);
    public DestroyEvent OnDestroy;

    public void TakeDamage(IDamageable damager, float damageTaken, Weapon.WeaponType weaponType)
    {
        if (health <= 0)
            return;

        if (weaponType != weakness)
            damageTaken = 0.0f;

        health -= damageTaken;

        DamagePopup.Create(damagePopupSpawn.position, damageTaken);

        if (health <= 0)
        {
            OnDestroy?.Invoke(this);
            Destroy(this.gameObject);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
