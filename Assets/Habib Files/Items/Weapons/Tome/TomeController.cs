using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class TomeController : WeaponController 
{
    #region Variables

    private float currentTomeCharge = 100f;
    private readonly float aimSpeed = 1f;

    [SerializeField] private bool IsAimConstant = true;

    private readonly string tomeChargeGain = "TomeChargeGain";
    private readonly string tomeChargeDrain = "TomeChargeDrain";

    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform beam;

    private List<IDamageable> enemiesHitList = new List<IDamageable>(); // Makes a list to keep track of which enemies were hit

    #endregion

    #region Start/Update Functions
    private void Start() { SetWeaponStats(); }

    protected void SetWeaponStats() {
        if (weapon == null) Debug.Log("No Weapon Set");
        else {
            CanAttack = weapon.CanAttack;
            IsAttacking = weapon.IsAttacking;
            
            currentTomeCharge = weapon.startingCharge;
        }
    }

    private void Update() {
        if (IsAttacking) {
            lineRenderer.SetPositions(new Vector3[] { projectileSpawn.position, owner.mouseWorldPosition });
        }
    }

    #endregion

    #region Attack Functions

    protected override void AttackStart() {
        ToggleCanAttack(); // false
        ToggleIsAttacking(); // true
        TogglePlayerAim(IsAimConstant, aimSpeed);

        CancelInvoke(tomeChargeGain);
        InvokeRepeating(tomeChargeDrain, 0, (1f / weapon.chargeDrainedRate)); // Invokes the func TomeDrain(), instantly once, then once every (1 sec/TomeChargeRate)
    }

    protected override void AttackEnd() {
        ToggleCanAttack(); // true
        ToggleIsAttacking(); // false
        TogglePlayerAim(IsAimConstant, aimSpeed);

        lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
        CancelInvoke(tomeChargeDrain);
        InvokeRepeating(tomeChargeGain, 0f, (1f / weapon.chargeGainedRate)); // Invokes the func TomeCharge(), instantly once, then once every (1 sec/TomeChargeRate)
    }

    private void TomeChargeDrain() {
        currentTomeCharge--;
        if (owner.IsOwner) { Attack(); }
        if (currentTomeCharge < 0) {
            currentTomeCharge = 0;
            CancelInvoke(tomeChargeDrain);
            Debug.Log("Out of Energy");
        }
        Debug.Log("Current tome charge: " + currentTomeCharge);
    }

    private void TomeChargeGain() {
        currentTomeCharge++;
        if (currentTomeCharge >= weapon.maxCharge) {
            currentTomeCharge = weapon.maxCharge;
            CancelInvoke(tomeChargeGain);
        } 
        Debug.Log("Current tome charge: " + currentTomeCharge);
    }

    private void Attack() {
        BeamCreate();
    }

    private void BeamCreate() {
        Vector3 point0 = projectileSpawn.position;
        Vector3 point1 = owner.mouseWorldPosition;
        Vector3 aimDir = (point1 - point0).normalized;
        Transform tempBeam = Instantiate(beam, point0, Quaternion.LookRotation(aimDir, Vector3.up));

        tempBeam.GetComponent<BeamFunction>().StartCoroutine("Create", weapon.damageValue);
        tempBeam.position = Vector3.Lerp(point0, point1, 0.5f);
        tempBeam.localScale = new Vector3(0.1f, 0.1f, Vector3.Distance(point0, point1));
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsAttacking) return;
        if (!other.TryGetComponent(out IDamageable damageable)) return;
        if (enemiesHitList.Contains(damageable)) return;

        damageable.TakeDamage(weapon.damageValue, weapon.weaponType);
        enemiesHitList.Add(damageable);
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
        owner.TriggerAim(aimTime); // Calculate Seconds to aim in
    }

    #endregion
}
