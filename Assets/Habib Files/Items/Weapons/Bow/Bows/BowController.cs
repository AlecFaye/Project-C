using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class BowController : WeaponController 
{
    public Weapon weapon;

    public float AttackingTime;
    public float AttackingCooldown;

    private float currentBowCharge = 0;

    private string bowCharge = "BowCharge";

    private void Start() { SetWeaponStats(); }
    private void SetWeaponStats() {
        if (weapon == null) Debug.Log("No Weapon Set");
        else {
            CanAttack = weapon.CanAttack;
            IsAttacking = weapon.IsAttacking;
            AttackingTime = weapon.attackingTime;
            AttackingCooldown = weapon.attackingCooldown;
            currentBowCharge = weapon.startingCharge;
        }
    }

    public override void AttackStart() {
        if (CanAttack && !IsAttacking && owner.IsOwner) { // Cut out -> owner.Grounded (Checked if player was grounded)
            CanAttack = false;
            IsAttacking = true;
            Debug.Log("Yett");
            owner.IsAttacking = true;
            owner.IsConstantAim = true;
            owner.TriggerAim(weapon.maxCharge / weapon.chargeGainedRate); // Calculate Seconds to aim in

            currentBowCharge = 0;
            InvokeRepeating(bowCharge, 0f, (1f / weapon.chargeGainedRate)); // Invokes the func BowCharge(), instantly once, then once every (1 sec/BowChargeRate)
        }
    }

    public override void AttackEnd() {
        Debug.Log("End Attack Pre If");
        CancelInvoke(bowCharge);
        if (!CanAttack && IsAttacking && owner.IsOwner && currentBowCharge > 0)
            Debug.Log("End Attack Post If");
            BowAttack(currentBowCharge);
    }

    private void BowCharge() {
        currentBowCharge++;
        if (currentBowCharge > weapon.maxCharge)
            currentBowCharge = weapon.maxCharge;
        Debug.Log("Current bow charge: " + currentBowCharge);
    }

    private void BowAttack(float chargeValue) {
        CanAttack = true;
        DisableIsAttacking();
        currentBowCharge = weapon.startingCharge;
        
        owner.IsConstantAim = false;
        owner.TriggerAim(0.5f); // Manual Seconds to aim out

        Vector3 aimDir = (owner.mouseWorldPosition - owner._projectileSpawn.position).normalized;
        Arrow arrow = weapon._arrowType;
        Transform tempArrow = Instantiate(arrow.arrowModel, owner._projectileSpawn.position, Quaternion.LookRotation(aimDir, Vector3.up));
        
        tempArrow.GetComponent<ArrowFunction>().Create(
            arrow.travelSpeed * (chargeValue / 100), // Speed of arrow (travelSpeed) * by charge value (0% - 100%)
            arrow.damageValue + (weapon.damageValue * (chargeValue / 100)) // Damage of arrow (Arrow Damage + [bow damage * charge value {0% - 100%}] = total damage
            );
    }

    private void DisableIsAttacking() {
        IsAttacking = false;
        //this.transform.parent.parent.GetComponent<HotbarController>().IsAttacking = false;
        owner.IsAttacking = false;
    }
}
