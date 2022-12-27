using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponController : MonoBehaviour
{
    #region Variables

    public ThirdPersonController owner;
    public HotbarController hotbar;

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
    [SerializeField] protected SliderBar weaponChargeBar;

    // Tome Specific Variables
    protected float currentTomeCharge = 100f;
    protected readonly string tomeChargeGain = "TomeChargeGain";
    protected readonly string tomeChargeDrain = "TomeChargeDrain";
    [SerializeField] protected Transform beam;
    [SerializeField] protected LineRenderer lineRenderer;

    // Bow Specific Variables
    protected float currentBowCharge = 0f;
    protected string bowCharge = "BowCharge";

    #endregion


    #region (Awake, OnEnable, Start, Update) Functions

    private void Awake() {
        //Debug.Log("Awake: " + this.name);
    }

    private void Start() {
        //Debug.Log("Start: " + this.name);
    }

    public virtual void OnEnable() {
        if (!hotbar) {
            hotbar = this.transform.parent.parent.GetComponent<HotbarController>();
            owner = hotbar.player;
            weaponChargeBar = owner.stats.weaponChargeBar;
            UpdateWeaponChargeBar(false); // false
            return;
        }
    }

    private void Update() { 
        if (CanAttack && !IsAttacking && IsHoldingAttack && !IsAnimating && weapon) AttackWindup();
    }

    #endregion

    #region Attack Functions

    public void OnAttackStart() {
        IsHoldingAttack = true;
        AttackWindup();
    }
    public void OnAttackRelease() {
        IsHoldingAttack = false;
        AttackEnd();
    }

    public virtual void AttackWindup() { Debug.Log("Start Attack Windup"); }
    public virtual void AttackStart() { Debug.Log("Start Attack Hitbox"); }
    public virtual void AttackStop() { Debug.Log("Stop Attack Hitbox"); }
    public virtual void AttackEnd() { Debug.Log("Start Attack Finished"); }


    #endregion

    #region Checks and Toggle Functions

    public bool SwitchableCheck() {
        if (!owner.IsOwner) return false;
        if (!CanAttack) return false;
        if (IsAttacking) return false;
        if (IsHoldingAttack) return false;
        if (IsAnimating) return false;

        return true;
    }

    public void ToggleIsAnimating(bool triggerAnim) { 
        IsAnimating = !IsAnimating;
        if (triggerAnim) owner._animator.SetTrigger(_animIDStartAttack); // Will call the currently selected weapon's attack animation
    }
    protected void ToggleCanAttack() { CanAttack = !CanAttack; }
    protected void ToggleIsAttacking() {
        IsAttacking = !IsAttacking;
        owner.IsAttacking = IsAttacking;
    }
    protected void TogglePlayerAim(bool isConstantAim, float aimTime = 1f) {
        if (isConstantAim) {
            owner.IsConstantAim = isConstantAim;
            owner.TriggerAim(aimTime, weapon.weaponType); // Calculate Seconds to aim in
        }
        if (!isConstantAim) {
            owner.IsConstantAim = isConstantAim;
            owner.aimTarget = owner.mouseWorldPosition;
        }
    }
    //public virtual void ToggleOwnerRig(bool turnOn) { Debug.Log("Update Player Rig to reflect this"); }

    #endregion

    #region Melee Functions

    private void OnTriggerEnter(Collider other) {
        if (!IsAttacking) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (enemiesHitList.Contains(damageable)) return;

        damageable.TakeDamage(owner.stats, weapon.damageValue, weapon.weaponType);
        enemiesHitList.Add(damageable);
    }

    #endregion

    #region Charge Functions

    private void BowCharge() {
        currentBowCharge++;
        if (currentBowCharge > weapon.maxCharge)
            currentBowCharge = weapon.maxCharge;

        weaponChargeBar.SetCurrentValue(currentBowCharge);
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

        weaponChargeBar.SetCurrentValue(currentTomeCharge);
        //Debug.Log("Current tome charge: " + currentTomeCharge);
    }

    private void TomeChargeGain() {
        currentTomeCharge++;
        if (currentTomeCharge >= weapon.maxCharge) {
            currentTomeCharge = weapon.maxCharge;
            CancelInvoke(tomeChargeGain);
        }
        
        weaponChargeBar.SetCurrentValue(currentTomeCharge);
        //Debug.Log("Current tome charge: " + currentTomeCharge);
    }

    #endregion

    #region Create Functions

    protected void CreateArrow() {
        Vector3 point0 = projectileSpawn.position;
        Vector3 point1 = owner.mouseWorldPosition;
        Vector3 aimDir = (point1 - point0).normalized;
        Arrow arrow = weapon._arrowType;
        Transform tempArrow = Instantiate(arrow.arrowModel, point0, Quaternion.LookRotation(aimDir, Vector3.up));

        tempArrow.GetComponent<ArrowFunction>().Create(
            owner.stats,
            arrow.travelSpeed * (currentBowCharge / weapon.maxCharge), // Speed of arrow (travelSpeed) * by charge value (0% - 100%)
            arrow.damageValue + (weapon.damageValue * (currentBowCharge / weapon.maxCharge)) // Damage of arrow (Arrow Damage + [bow damage * charge value {0% - 100%}] = total damage
            );

        currentBowCharge = weapon.startingCharge;
    }
    private void BeamCreate() {
        Vector3 point0 = projectileSpawn.position;
        Vector3 point1 = owner.mouseWorldPosition;
        Vector3 aimDir = (point1 - point0).normalized;
        var rotation = Quaternion.LookRotation(aimDir, Vector3.up);
        rotation *= Quaternion.Euler(90f, 0f, 0f); // this adds a 90 degrees Y rotation
        Transform tempBeam = Instantiate(beam, point0, rotation);

        tempBeam.TryGetComponent(out BeamFunction beamFunction);
        StartCoroutine(beamFunction.Create(owner.stats, weapon.damageValue));
        tempBeam.position = Vector3.Lerp(point0, point1, 0.5f);
        tempBeam.localScale = new Vector3(0.1f, Vector3.Distance(point0, point1) / 2, 0.1f);
    }

    #endregion

    #region Bar Functions

    public void UpdateWeaponChargeBar(bool IsActive) {
        weaponChargeBar.ToggleHide(IsActive);

        if (IsActive) {
            weaponChargeBar.SetMaxValue(weapon.maxCharge);
            weaponChargeBar.SetCurrentValue(weapon.startingCharge);
        }
    }

    #endregion

}
