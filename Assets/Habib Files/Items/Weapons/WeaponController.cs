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

    #endregion

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
