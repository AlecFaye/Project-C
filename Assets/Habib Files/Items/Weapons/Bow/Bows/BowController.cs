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

    private float currentBowCharge = 0;

    private string bowCharge = "BowCharge";

    [SerializeField] private Transform projectileSpawn;

    #endregion

    #region Start Functions

    private void Start() { SetWeaponStats(); }
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
            CreateArrow();
        }
    }

    #region Charge Functions

    private void BowCharge() {
        currentBowCharge++;
        if (currentBowCharge > weapon.maxCharge)
            currentBowCharge = weapon.maxCharge;
        Debug.Log("Current bow charge: " + currentBowCharge);
    }

    #endregion

    private void CreateArrow() {
        Vector3 point0 = projectileSpawn.position;
        Vector3 point1 = owner.mouseWorldPosition;
        Vector3 aimDir = (point1 - point0).normalized;
        Arrow arrow = weapon._arrowType;
        Transform tempArrow = Instantiate(arrow.arrowModel, point0, Quaternion.LookRotation(aimDir, Vector3.up));

        tempArrow.GetComponent<ArrowFunction>().Create(
            arrow.travelSpeed * (currentBowCharge / weapon.maxCharge), // Speed of arrow (travelSpeed) * by charge value (0% - 100%)
            arrow.damageValue + (weapon.damageValue * (currentBowCharge / weapon.maxCharge)) // Damage of arrow (Arrow Damage + [bow damage * charge value {0% - 100%}] = total damage
            );

        currentBowCharge = weapon.startingCharge;
    }

    #endregion

    #region Toggle Functions

    private void ToggleIsAttacking() {
        IsAttacking = !IsAttacking;
        owner.IsAttacking = IsAttacking;
    }
    private void ToggleCanAttack() { CanAttack = !CanAttack; }
    private void TogglePlayerAim(bool isConstantAim, float aimTime) {
        owner.IsConstantAim = isConstantAim;
        owner.TriggerAim(aimTime, weapon.weaponType); // Calculate Seconds to aim in

        UpdateOwnerRig();
    }

    public override void UpdateOwnerRig() {
        owner.Body.data.offset = new Vector3(100f, 0f, 0f);

        owner.RightArm.weight = 1;
        
    }

    #endregion
}
