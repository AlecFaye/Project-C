using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class AxeController : WeaponController 
{
    #region Variables

    [SerializeField] private bool IsAimConstant = false;

    [SerializeField] private TrailRenderer trailRenderer;

    #endregion

    #region Enable Functions

    private void OnEnable() {
        SetWeaponStats();
        UpdateWeaponChargeBar(false); // false
    }
    private void SetWeaponStats() {
        if (weapon == null) Debug.Log("No Weapon Set");
        else {
            CanAttack = weapon.CanAttack;
            IsAttacking = weapon.IsAttacking;
 
            _animIDStartAttack = "Axe Attack";
        }
    }

    #endregion

    #region Attack Functions

    public override void AttackWindup() {
        if (CanAttack && !IsAttacking && owner.IsOwner) {
            ToggleIsAnimating(true); // true
            ToggleCanAttack(); // false
            TogglePlayerAim(IsAimConstant);
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

    public override void AttackEnd() {
        if (!IsAnimating) {
            enemiesHitList = new List<IDamageable>(); // Resets the list of enemies so that they can be hit again
            ToggleCanAttack(); // true
        }
    }

    #endregion

    #region Toggle Functions

    private void ToggleTrailRenderer() { trailRenderer.emitting = !trailRenderer.emitting; }

    #endregion
}
