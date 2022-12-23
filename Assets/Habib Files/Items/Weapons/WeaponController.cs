using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region Variables

    public ThirdPersonController owner;

    public Transform ref_RightHand;
    public Transform ref_LeftHand;

    [SerializeField] protected Weapon weapon;

    public bool CanAttack = true;
    public bool IsAttacking = false;
    public bool IsHoldingAttack = false;

    // Animation Variables
    protected string _animIDStartAttack = "Start Attack";

    protected bool IsAnimating = false;

    protected const int axeAttackLayer = 1;
    protected const int bowAttackLayer = 2;
    protected const int pickaxeAttackLayer = 3;
    protected const int tomeAttackLayer = 4;

    #endregion

    private void Update() { if (CanAttack && !IsAttacking && IsHoldingAttack && weapon) AttackStart(); }

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

    public void ToggleIsAnimating() { IsAnimating = !IsAnimating; } // Called by the owner

}
