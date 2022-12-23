using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class AxeController : WeaponController 
{
    #region Variables

    private float attackingTime;
    private float attackingCooldown;

    [SerializeField] private bool IsAimConstant = false;

    [SerializeField] private TrailRenderer trailRenderer;

    private List<IDamageable> enemiesHitList = new List<IDamageable>(); // Makes a list to keep track of which enemies were hit

    #endregion

    #region Start Functions

    private void Start() { SetWeaponStats(); }

    private void SetWeaponStats() {
        if (weapon == null) Debug.Log("No Weapon Set");
        else {
            CanAttack = weapon.CanAttack;
            IsAttacking = weapon.IsAttacking;
            attackingTime = weapon.attackingTime;
            attackingCooldown = weapon.attackingCooldown;
        }
    }

    #endregion

    #region Attack Functions

    public override void AttackWindup() {
        if (CanAttack && !IsAttacking && owner.IsOwner) {
            ToggleIsAnimating(); // true
            ToggleCanAttack(); // false
            TogglePlayerAim(IsAimConstant);
            owner._animator.SetLayerWeight(axeAttackLayer, 1);
            owner._animator.SetTrigger(_animIDStartAttack); // Will call the currently selected weapon's attack animation
        }
    }
    public override void AttackStart() {
        ToggleIsAttacking(); // true
        ToggleTrailRenderer();
    }
    public override void AttackStop() {
        ToggleTrailRenderer();
        ToggleIsAttacking(); // false
    }
    // This will be called if the button is released again Fix later
    public override void AttackEnd() {
        if (!IsAnimating) {
            owner._animator.SetLayerWeight(axeAttackLayer, 0);
            enemiesHitList = new List<IDamageable>(); // Resets the list of enemies so that they can be hit again
            ToggleCanAttack(); // true
        }
    }


    private void OnTriggerEnter(Collider other) {
        if (!IsAttacking) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (enemiesHitList.Contains(damageable)) return;

        damageable.TakeDamage(weapon.damageValue, weapon.weaponType);
        enemiesHitList.Add(damageable);
    }

    #endregion

    #region Toggle Functions

    private void ToggleCanAttack() { CanAttack = !CanAttack; }
    private void ToggleIsAttacking() {
        IsAttacking = !IsAttacking;
        owner.IsAttacking = IsAttacking;
    }
    private void TogglePlayerAim(bool isConstantAim) {
        owner.aimTarget = owner.mouseWorldPosition;
        owner.IsConstantAim = isConstantAim;
    }
    private void ToggleTrailRenderer() { trailRenderer.emitting = !trailRenderer.emitting; }

    #endregion
}
