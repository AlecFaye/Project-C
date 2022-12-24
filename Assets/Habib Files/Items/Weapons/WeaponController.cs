using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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

    // Ranged Specific Variables
    [SerializeField] protected Transform projectileSpawn;

    // Tome Specific Variables
    protected float currentTomeCharge = 100f;
    protected readonly string tomeChargeGain = "TomeChargeGain";
    protected readonly string tomeChargeDrain = "TomeChargeDrain";
    [SerializeField] protected Transform beam;
    [SerializeField] protected LineRenderer lineRenderer;

    // Bow Specific Variables
    protected float currentBowCharge = 0;
    protected string bowCharge = "BowCharge";

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



    #region Charge Functions

    private void BowCharge() {
        currentBowCharge++;
        if (currentBowCharge > weapon.maxCharge)
            currentBowCharge = weapon.maxCharge;
        //Debug.Log("Current bow charge: " + currentBowCharge);
    }

    private void TomeChargeDrain() {
        currentTomeCharge--;
        if (owner.IsOwner) { BeamCreate(); }
        if (currentTomeCharge < 0) {
            currentTomeCharge = 0;
            CancelInvoke(tomeChargeDrain);
            lineRenderer.SetPositions(new Vector3[] { projectileSpawn.position, projectileSpawn.position });
            Debug.Log("Out of Energy");
        }
        //Debug.Log("Current tome charge: " + currentTomeCharge);
    }

    private void TomeChargeGain() {
        currentTomeCharge++;
        if (currentTomeCharge >= weapon.maxCharge) {
            currentTomeCharge = weapon.maxCharge;
            CancelInvoke(tomeChargeGain);
        }
        //Debug.Log("Current tome charge: " + currentTomeCharge);
    }

    #endregion

    private void BeamCreate() {
        Vector3 point0 = projectileSpawn.position;
        Vector3 point1 = owner.mouseWorldPosition;
        Vector3 aimDir = (point1 - point0).normalized;
        var rotation = Quaternion.LookRotation(aimDir, Vector3.up);
        rotation *= Quaternion.Euler(90f, 0f, 0f); // this adds a 90 degrees Y rotation
        Transform tempBeam = Instantiate(beam, point0, rotation);

        tempBeam.GetComponent<BeamFunction>().StartCoroutine("Create", weapon.damageValue);
        tempBeam.position = Vector3.Lerp(point0, point1, 0.5f);
        tempBeam.localScale = new Vector3(0.1f, Vector3.Distance(point0, point1) / 2, 0.1f);
    }

}
