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

    [SerializeField] private string attackTrigger = "Axe Attack";

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

    protected override void AttackStart() {
        if (CanAttack && !IsAttacking && owner.IsOwner) {
            ToggleCanAttack(); // false
            ToggleIsAttacking(); // true
            TogglePlayerAim(IsAimConstant);
            StartCoroutine(Attack());
        }
    }

    protected override void AttackEnd() {
        return;
    }

    private IEnumerator Attack() {
        ToggleTrailRenderer();
        owner._animator.SetTrigger(attackTrigger); // Will call the currently selected weapon's attack animation

        yield return new WaitForSeconds(attackingTime);
        ToggleTrailRenderer();
        ToggleIsAttacking(); // false

        yield return new WaitForSeconds(attackingCooldown);
        enemiesHitList = new List<IDamageable>(); // Resets the list of enemies so that they can be hit again
        ToggleCanAttack(); // true
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

    private void ToggleIsAttacking() {
        IsAttacking = !IsAttacking;
        owner.IsAttacking = IsAttacking;
    }
    private void ToggleCanAttack() { CanAttack = !CanAttack; }
    private void TogglePlayerAim(bool isConstantAim) {
        owner.IsConstantAim = isConstantAim;
        owner.aimTarget = owner.mouseWorldPosition;
        owner.RotatePlayerToCamera();
    }
    private void ToggleTrailRenderer() { trailRenderer.emitting = !trailRenderer.emitting; }

    #endregion
}
