using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region Variables

    public ThirdPersonController owner;

    public Transform ref_LeftHand;
    public Transform ref_RightHand;

    public Weapon weapon;

    public bool CanAttack = true;
    public bool IsAttacking = false;
    public bool IsHoldingAttack = false;

    // Animation Variables
    protected string _animIDStartAttack = "None Attack";

    protected bool IsAnimating = false;

    // Melee Specific Variables
    protected List<IDamageable> enemiesHitList = new List<IDamageable>(); // Makes a list to keep track of which enemies were hit

    #endregion

    private void Update() { if (CanAttack && !IsAttacking && IsHoldingAttack && !IsAnimating && weapon) AttackWindup(); }

    #region Attack Start and End

    public void OnAttackStart() {
        IsHoldingAttack = true;
        AttackWindup();
    }
    public void OnAttackRelease() {
        IsHoldingAttack = false;
        AttackEnd();
    }

    #endregion

    public virtual void AttackWindup() { Debug.Log("Start Attack Windup"); }
    public virtual void AttackStart() { Debug.Log("Start Attack Hitbox"); }
    public virtual void AttackStop() { Debug.Log("Stop Attack Hitbox"); }
    public virtual void AttackEnd() { Debug.Log("Start Attack Finished"); }

    public virtual void ToggleOwnerRig(bool turnOn) { Debug.Log("Update Player Rig to reflect this"); }
    
    public bool SwitchableCheck() {
        if (!owner.IsOwner) return false;
        if (!CanAttack) return false;
        if (IsAttacking) return false;
        if (IsHoldingAttack) return false;
        if (IsAnimating) return false;

        else return true;
    }
    
    public void ToggleIsAnimating(bool triggerAnim) { 
        IsAnimating = !IsAnimating;
        if (triggerAnim) owner._animator.SetTrigger(_animIDStartAttack); // Will call the currently selected weapon's attack animation
    }


    #region Melee Functions

    private void OnTriggerEnter(Collider other) {
        if (!IsAttacking) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (enemiesHitList.Contains(damageable)) return;

        damageable.TakeDamage(weapon.damageValue, weapon.weaponType);
        enemiesHitList.Add(damageable);
    }

    #endregion

}
