using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class AxeController : WeaponController 
{

    public float AttackingTime;
    public float AttackingCooldown;

    [SerializeField] private TrailRenderer trailRenderer;

    private List<Collider> enemiesHitList = new List<Collider>(); // Makes a list to keep track of which enemies were hit (enemies added by the CollisionDetection Script on weapons)


    private void Start() {
        SetWeaponStats();
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

    #region Attack Functions

    public override void AttackStart() {
        if (CanAttack && !IsAttacking && owner.IsOwner) {
            ToggleCanAttack(false);
            ToggleIsAttacking(true);
            TogglePlayerAim();
            StartCoroutine(AxeAttack());
        }
    }

    public override void AttackEnd() {
        return;
    }

    private IEnumerator AxeAttack() {
        owner._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
        ToggleTrailRenderer();

        yield return new WaitForSeconds(AttackingTime);
        ToggleIsAttacking(false);
        ToggleTrailRenderer();

        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        ToggleCanAttack(true);
    }

    #endregion

    private void OnTriggerEnter(Collider other) {
        // Checks if it collided with an enemy ====== Checks if the player should be attacking rn(done in weapon controller) ====== Checks if the enemy was already hit by this attack
        if (weapon.IsAttacking && enemiesHitList.Contains(other)) {
            if (other.TryGetComponent(out IDamageable damageable)) {
                damageable.TakeDamage(weapon.damageValue, weapon.weaponType);
                enemiesHitList.Add(other);
            }
        }
    }

    #region Toggle Functions

    private void ToggleIsAttacking(bool state) {
        IsAttacking = state;
        owner.IsAttacking = state;

        hotbarController.UpdateAttackingStates();
    }
    private void ToggleCanAttack(bool state) {
        CanAttack = state;
        
        hotbarController.UpdateAttackingStates();
    }
    private void TogglePlayerAim() {
        owner.IsConstantAim = false;
        owner.aimTarget = owner.mouseWorldPosition;
        owner.RotatePlayerToCamera();
    }
    private void ToggleTrailRenderer() { trailRenderer.emitting = !trailRenderer.emitting; }

    #endregion
}
