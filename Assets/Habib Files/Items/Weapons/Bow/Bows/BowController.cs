using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class BowController : WeaponController 
{
    #region Variables

    private bool IsAimConstant = true;

    #endregion

    #region Enable Functions

    public override void OnEnable() {
        base.OnEnable();

        SetWeaponStats();
        UpdateWeaponChargeBar(true); // true
    }
    private void SetWeaponStats() {
        if (weapon == null) Debug.Log("No Weapon Set");
        else {
            CanAttack = weapon.CanAttack;
            IsAttacking = weapon.IsAttacking;
            
            currentBowCharge = weapon.startingCharge;
            _animIDStartAttack = "Bow Attack";
        }
    }

    #endregion

    #region Attack Functions

    public override void AttackWindup() {
        if (CanAttack && !IsAttacking && !IsAnimating && owner.IsOwner){
            ToggleIsAnimating(true); // true
            ToggleCanAttack(); // false
            currentBowCharge = 0;
        }
    }

    public override void AttackStart() {
        ToggleIsAttacking(); // true
        TogglePlayerAim(IsAimConstant, weapon.maxCharge / weapon.chargeGainedRate);
        //ToggleOwnerRig(true); // true
        InvokeRepeating(bowCharge, 0f, (1f / weapon.chargeGainedRate)); // Invokes the func BowCharge(), instantly once, then once every (1 sec/BowChargeRate)
    }
    public override void AttackStop() { 
        Debug.Log("Uneeded Funtion For Bow"); 
    }

    public override void AttackEnd() {
        CancelInvoke(bowCharge);
        if (!CanAttack && IsAttacking && owner.IsOwner && currentBowCharge > 0) { 
            ToggleIsAnimating(true); // false
            ToggleCanAttack(); // true
            ToggleIsAttacking(); // false
            TogglePlayerAim(IsAimConstant, 0.5f);
            //ToggleOwnerRig(false); // false
            CreateArrow();
            UpdateWeaponChargeBar(true);
        }
    }

    #endregion

    #region Toggle Functions



    #endregion
}
