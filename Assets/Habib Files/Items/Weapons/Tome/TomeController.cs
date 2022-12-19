using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class TomeController : WeaponController 
{
    public Weapon weapon;

    public bool CanAttack;
    public bool IsAttacking;
    public float AttackingTime;
    public float AttackingCooldown;
    private float currentTomeCharge = 100;

    [SerializeField] private TrailRenderer trail;

    private List<Collider> enemiesHitList = new List<Collider>(); // Makes a list to keep track of which enemies were hit (enemies added by the CollisionDetection Script on weapons)


    private void Start() { SetWeaponStats(); }
    private void SetWeaponStats() {
        if (weapon == null) Debug.Log("No Weapon Set");
        else {
            CanAttack = weapon.CanAttack;
            IsAttacking = weapon.IsAttacking;
            AttackingTime = weapon.attackingTime;
            AttackingCooldown = weapon.attackingCooldown;
            currentTomeCharge = weapon.startingCharge;
        }
    }

    public override void AttackStart() {
        if (CanAttack && !IsAttacking && owner.IsOwner) {
            CanAttack = false;
            IsAttacking = true;

            owner.IsAttacking = true;
            owner.IsConstantAim = true;
            owner.TriggerAim(weapon.maxCharge / weapon.chargeGainedRate); // Calculate Seconds to aim in

            TomeAttack();
        }
    }

    public override void AttackEnd() {
        Debug.Log("End Attack");
    }

    private void TomeAttack() {
        CanAttack = false;
        IsAttacking = true;

        owner._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation

        DisableIsAttacking();

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
        IsAttacking = false;
        //this.transform.parent.parent.GetComponent<HotbarController>().IsAttacking = false;
        owner.IsAttacking = false;
    }
}
