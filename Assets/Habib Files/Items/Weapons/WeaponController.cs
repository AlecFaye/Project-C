using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Weapon;


public class WeaponController : MonoBehaviour
{
    public ThirdPersonController player; // Refrences the player it's attatched too

    [SerializeField] private Transform[] HotbarSlots;
    [SerializeField] private Transform[] Hotbar; // this is for testing

    private int selectedWeapon = 0;

    private Weapon currentWeapon;


    //Stats -> Get overwritten
    //private bool CanAttack;
    //public bool IsAttacking; // Is public to be called by other functions
    //private float AttackingTime;
    //private float AttackingCooldown;

    // Channel Variables
    private bool IsChannelingAttack = false; // Used to check if player should be charging their attack
    private float currentBowCharge = 0;
    private float currentTomeCharge = 100;
    private float tomeChargedFor = 0;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform beamHitbox;


    private void Start()
    {
        int hotbarSlot = 0;
        foreach (Transform weapon in Hotbar) {
            if (weapon != null) {
                CreateWeapon(weapon, hotbarSlot);
            }
            hotbarSlot++;
        }

        SelectWeapon();
    }

    // Fix the bug so items can go in any positions (Maybe make a child for each Hotbar slot and add the weapons to the children) <- [This solution allows me to add back the line renderer as a child of the weapon controller]
    private void CreateWeapon(Transform weapon, int hotbarSlot)
    {
        Transform tempWeapon = Instantiate(weapon, this.transform.position, Quaternion.identity); // Creates the weapon in the hotbar slot
        tempWeapon.transform.SetParent(HotbarSlots[hotbarSlot]); // Sets this gameobject to the parent of the 
        tempWeapon.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Sets rotation to hand
        SetOwnerofWeapon(weapon); // Set Player of weapon to weapon
    }

    private void SetOwnerofWeapon(Transform weapon) {
        Debug.Log("Yeet");
        if (weapon.GetComponent<AxeController>() != null)
            weapon.GetComponent<AxeController>().Create(player);
    }

    private void SelectWeapon()
    {
        //int i = 0;
        //foreach (Transform weapon in this.transform)
        //{
        //    if (i == selectedWeapon)
        //    {
        //        weapon.gameObject.SetActive(true);
        //        currentWeapon = Hotbar[selectedWeapon];
        //        Transform currentWeaponObject = weapon;

        //        CanAttack = currentWeapon.CanAttack;
        //        IsAttacking = currentWeapon.IsAttacking;
        //        AttackingTime = currentWeapon.attackingTime;
        //        AttackingCooldown = currentWeapon.attackingCooldown;
        //    }
        //    else
        //        weapon.gameObject.SetActive(false);
        //    i++;
        //}
    }

    private void Update() {
        if (IsChannelingAttack && currentWeapon.weaponType == WeaponType.Tome) {
           lineRenderer.SetPositions(new Vector3[] { player._projectileSpawn.position, player.mouseWorldPosition});
        }
    }

    #region On Attack Input
    //public void OnAttack()
    //{
    //    switch (Hotbar[selectedWeapon].weaponType)
    //    {
    //        case WeaponType.None:
    //            Debug.Log("Wait stop should be NONE");
    //            break;


    //        case WeaponType.Bow:
    //            if (!IsChannelingAttack) {
    //                player.IsAttacking = true;
    //                player.IsConstantAim = true;
    //                player.TriggerAim(0.5f); // Calculate Seconds to aim in
                    
    //                IsChannelingAttack = true;
    //                currentBowCharge = 0;
    //                InvokeRepeating("BowCharge", 0f, (1f / currentWeapon.chargeGainedRate)); // Invokes the func BowCharge(), instantly once, then once every (1 sec/BowChargeRate)

    //            }
    //            else {
    //                IsChannelingAttack = false;
    //                CancelInvoke("BowCharge");
    //                if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
    //                    StartCoroutine(BowAttack(currentBowCharge));
    //            }
    //            break;

    //        case WeaponType.Pickaxe:
    //            if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner) {
    //                player.IsAttacking = true;
    //                player.IsConstantAim = false;
    //                player.aimTarget = player.mouseWorldPosition;
    //                player.RotatePlayerToCamera();

    //                StartCoroutine(PickaxeAttack());
    //            }
    //            break;

    //        case WeaponType.Tome:
    //            if (!IsChannelingAttack) {
    //                player.IsAttacking = true;
    //                player.IsConstantAim = true;

    //                IsChannelingAttack = true;
    //                tomeChargedFor = 0;
    //                CancelInvoke("TomeCharge");
    //                InvokeRepeating("TomeDrain", 0, (1f / currentWeapon.chargeLostRate)); // Invokes the func TomeDrain(), instantly once, then once every (1 sec/TomeChargeRate)
    //            }
    //            else {
    //                player.IsAttacking = false;
    //                player.IsConstantAim = false;

    //                IsChannelingAttack = false;
    //                lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
    //                CancelInvoke("TomeDrain");
    //                InvokeRepeating("TomeCharge", 0f, (1f / currentWeapon.chargeLostRate)); // Invokes the func TomeCharge(), instantly once, then once every (1 sec/TomeChargeRate)
    //            }
    //            CanAttack = !IsChannelingAttack;
    //            IsAttacking = IsChannelingAttack;
    //            break;
    //    }
    //}
    #endregion

    #region Charge Functions
    private void BowCharge()
    {
        currentBowCharge++;
        if (currentBowCharge > currentWeapon.Max_Charge) currentBowCharge = currentWeapon.Max_Charge;
        //Debug.Log("Current bow charge: " + currentBowCharge);
    }

    private void TomeDrain()
    {
        currentTomeCharge--;
        tomeChargedFor++;
        //Debug.Log("Current tome charge: " + currentTomeCharge);
        Debug.Log("Current tome charged for: " + tomeChargedFor);
        if (tomeChargedFor % 5 == 0) {
            tomeChargedFor = 0;
            //if (player.Grounded && player.IsOwner)
                //StartCoroutine(TomeAttack());
                //TomeAttack();
        }
        if (currentTomeCharge < 0) {
            currentTomeCharge = 0;
            CancelInvoke("TomeDrain");
            Debug.Log("Out of Energy");
        }
    }

    private void TomeCharge()
    {
        currentTomeCharge++;
        if (currentTomeCharge >= currentWeapon.Max_Held_Charge)
        {
            currentTomeCharge = currentWeapon.Max_Held_Charge;
            CancelInvoke("TomeCharge");
        }
        //Debug.Log("Current tome charge: " + currentTomeCharge);
    }
    #endregion

    //#region Weapon Attack Functions
    //private IEnumerator BowAttack(float chargeValue) {
    //    CanAttack = false;
    //    IsAttacking = true;

    //    player.IsAttacking = false;
    //    player.IsConstantAim = false; 
    //    player.TriggerAim(0.5f); // Calculate Seconds to aim out


    //    Vector3 aimDir = (player.mouseWorldPosition - player._projectileSpawn.position).normalized;
    //    Transform tempArrow = Instantiate(currentWeapon._arrowType.arrowModel, player._projectileSpawn.position, Quaternion.LookRotation(aimDir, Vector3.up));
    //    tempArrow.GetComponent<ArrowFunction>().Create(
    //        currentWeapon._arrowType.travelSpeed * (currentBowCharge / 100), // Speed of arrow (travelSpeed) * by charge value (0% - 100%)
    //        (currentWeapon._arrowType.damageValue + (currentWeapon.damageValue * (currentBowCharge / 100))) // Damage of arrow (Arrow Damage + [bow damage * charge value {0% - 100%}] = total damage
    //        );

    //    yield return new WaitForSeconds(AttackingTime);
    //    IsAttacking = false;
    //    yield return new WaitForSeconds(AttackingCooldown);
    //    CanAttack = true;
    //}
    //private IEnumerator PickaxeAttack() {
    //    CanAttack = false;
    //    IsAttacking = true;

    //    player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
       
    //    yield return new WaitForSeconds(AttackingTime);
    //    IsAttacking = false;
    //    player.IsAttacking = false;
    //    yield return new WaitForSeconds(AttackingCooldown);
    //    //enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
    //    CanAttack = true;
    //}
    //private void TomeAttack() {
    //    Vector3 point0 = player._projectileSpawn.position;
    //    Vector3 point1 = player.mouseWorldPosition;
    //    Vector3 aimDir = (point1 - point0).normalized;
    //    Transform tempBeam = Instantiate(beamHitbox, point0, Quaternion.LookRotation(aimDir, Vector3.up));
        
    //    tempBeam.GetComponent<BeamFunction>().StartCoroutine("Create", currentWeapon.damageValue);
    //    tempBeam.position = Vector3.Lerp(point0, point1, 0.5f);
    //    tempBeam.localScale = new Vector3(0.1f, 0.1f, Vector3.Distance(point0, point1));
    //}
    //#endregion

    //#region Hotbar Inputs

    //private void SwitchHotBar(int hotbarNum) {
    //    if (player.IsOwner && CanAttack && !IsChannelingAttack) {
    //        selectedWeapon = hotbarNum - 1; // - 1 for arrary starting at 0
    //        SelectWeapon();
    //        //Debug.Log(Hotbar[selectedWeapon].weaponName);
    //    }
    //}
    //private void OnHotbar1() { SwitchHotBar(1); }
    //private void OnHotbar2() { SwitchHotBar(2); }
    //private void OnHotbar3() { SwitchHotBar(3); }
    //private void OnHotbar4() { SwitchHotBar(4); }
    //private void OnHotbar5() { SwitchHotBar(5); }
    //#endregion
}