using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class AxeController : WeaponController 
{
    public Weapon weapon;

    public bool CanAttack;
    public bool IsAttacking;
    public float AttackingTime;
    public float AttackingCooldown;

    [SerializeField] private TrailRenderer trail;

    private List<Collider> enemiesHitList = new List<Collider>(); // Makes a list to keep track of which enemies were hit (enemies added by the CollisionDetection Script on weapons)


    private void Start() {
        SetWeaponStats();
        Debug.Log(owner);
    }
    private void SetWeaponStats() {
        if (weapon == null) Debug.Log("No Weapon Set");
        else {
            CanAttack = weapon.CanAttack;
            IsAttacking = weapon.IsAttacking;
            AttackingTime = weapon.attackingTime;
            AttackingCooldown = weapon.attackingCooldown;
        }
    }

    public override void Attack()
    {
        if (!IsAttacking && owner.IsOwner) { // Cut out -> owner.Grounded (Checked if player was grounded)
            if (CanAttack) {
                owner.IsAttacking = true;
                owner.IsConstantAim = false;
                owner.aimTarget = owner.mouseWorldPosition;
                owner.RotatePlayerToCamera();

                StartCoroutine(AxeAttack());
            } else {
                DisableIsAttacking();
            }
        }
    }

    private IEnumerator AxeAttack() {
        CanAttack = false;
        IsAttacking = true;

        owner._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation

        yield return new WaitForSeconds(AttackingTime);
        IsAttacking = false;
        owner.IsAttacking = false;
        DisableIsAttacking();

        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        CanAttack = true;
    }

    private void OnTriggerEnter(Collider other) {
        // Checks if it collided with an enemy ====== Checks if the player should be attacking rn(done in weapon controller) ====== Checks if the enemy was already hit by this attack
        if (weapon.IsAttacking && enemiesHitList.Contains(other)) {
            if (other.TryGetComponent(out IDamageable damageable)) {
                damageable.TakeDamage(weapon.damageValue, weapon.weaponType);
                enemiesHitList.Add(other);
            }
        }
    }

    private void DisableIsAttacking() {
        this.transform.parent.parent.GetComponent<InventoryController>().IsAttacking = false;
    }
}
