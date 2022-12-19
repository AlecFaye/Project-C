using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region Variables

    public ThirdPersonController owner;

    [SerializeField] protected Weapon weapon;

    protected bool CanAttack = true; // Used to check if player can attack
    protected bool IsAttacking = false; // Used to check if player is attacking
    protected bool IsHoldingAttack = false; // Used to check if player holding the button

    #endregion

    //public HotbarController hotbarController;

    private void Update() { if (CanAttack && !IsAttacking && IsHoldingAttack && weapon) AttackStart(); }

    #region Attack Start and End

    public void OnAttackStart() {
        IsHoldingAttack = true;
        AttackStart();
    }
    public void OnAttackEnd() {
        IsHoldingAttack = false;
        AttackEnd();
    }

    #endregion

    protected virtual void AttackStart() { Debug.Log("Reeeee No Attack Start Set"); }
    protected virtual void AttackEnd() { Debug.Log("No Attack End Set"); }
}
