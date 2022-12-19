using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class HotbarController : MonoBehaviour
{
    public ThirdPersonController player; // Refrences the player it's attatched too
    public WeaponController currentWeapon; // Refrences the current weapon's WeaponController Script

    [SerializeField] private Transform[] HotbarSlots;
    private int selectedWeapon = 0;

    private bool CanAttack = true; // Used to check if player can attack
    private bool IsAttacking = false; // Used to check if player is attacking
    
    private bool IsHoldingAttack = false; // Used to check if player holding the button

    

    [SerializeField] private Transform[] Hotbar; // this is for testing

    //private StarterAssetsInputs starterAssetsInputs;
    //private InputActionReference inputActionReference;
    //private Weapon currentWeapon;
    // Channel Variables
    //private float currentTomeCharge = 100;
    //private float tomeChargedFor = 0;
    //[SerializeField] private LineRenderer lineRenderer;
    //[SerializeField] private Transform beamHitbox;

    //private void Awake() {
    //    starterAssetsInputs = player.GetComponent<StarterAssetsInputs>();
    //}

    private void Start() {
        int hotbarSlot = 0;
        foreach (Transform weapon in Hotbar) {
            if (weapon != null) CreateWeapon(weapon, hotbarSlot);
            hotbarSlot++;
        } 
        
        SelectWeapon();
    }
    private void Update() {
        //if (IsChannelingAttack && currentWeapon.weaponType == WeaponType.Tome)
        //{
        //    lineRenderer.SetPositions(new Vector3[] { player._projectileSpawn.position, player.mouseWorldPosition });
        //}
    }

    private void CreateWeapon(Transform weapon, int hotbarSlot) {
        Transform tempWeapon = Instantiate(weapon, this.transform.position, Quaternion.identity); // Creates the weapon in the hotbar slot
        tempWeapon.transform.SetParent(HotbarSlots[hotbarSlot]); // Sets this gameobject to the parent of the 
        tempWeapon.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Sets rotation to hand
        
        SetOwnerofWeapon(tempWeapon); // Set Player of weapon to weapon
    }

    private void SetOwnerofWeapon(Transform weapon) {
        if (weapon.TryGetComponent<WeaponController>(out WeaponController weaponController)) {
            weaponController.owner = player;
            //weaponController.hotbarController = this;
        }
    }

    private void SelectWeapon() {
        int position = 0;
        foreach (Transform slot in HotbarSlots) {
            if (position == selectedWeapon) {
                slot.gameObject.SetActive(true);
                //currentWeapon = slot.GetChild(0).GetComponent<WeaponController>();
            }
            else
                slot.gameObject.SetActive(false);
            position++;
        }
    }
   
    //public void UpdateAttackingStates() {
    //    CanAttack = currentWeapon.CanAttack;
    //    IsAttacking = currentWeapon.IsAttacking;
    //}

    #region Hotbar Inputs

    private void SwitchHotBar(int hotbarNum) {
        if (player.IsOwner && CanAttack && !IsAttacking && !IsHoldingAttack) {
            selectedWeapon = hotbarNum;
            SelectWeapon();
        }
    }
    private void OnHotbar1() { SwitchHotBar(0); }
    private void OnHotbar2() { SwitchHotBar(1); }
    private void OnHotbar3() { SwitchHotBar(2); }
    private void OnHotbar4() { SwitchHotBar(3); }
    private void OnHotbar5() { SwitchHotBar(4); }
    private void OnHotbar6() { SwitchHotBar(5); }

    #endregion




    #region On Attack Input
    //public void OnAttack() {
    //    switch (Hotbar[selectedWeapon].weaponType) {
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

    //            } else {
    //                IsChannelingAttack = false;
    //                CancelInvoke("BowCharge");
    //                if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
    //                    StartCoroutine(BowAttack(currentBowCharge));
    //            }
    //            break;

    //        case WeaponType.Pickaxe:
    //            if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
    //            {
    //                player.IsAttacking = true;
    //                player.IsConstantAim = false;
    //                player.aimTarget = player.mouseWorldPosition;
    //                player.RotatePlayerToCamera();

    //                StartCoroutine(PickaxeAttack());
    //            }
    //            break;

    //        case WeaponType.Tome:
    //            if (!IsChannelingAttack)
    //            {
    //                player.IsAttacking = true;
    //                player.IsConstantAim = true;

    //                IsChannelingAttack = true;
    //                tomeChargedFor = 0;
    //                CancelInvoke("TomeCharge");
    //                InvokeRepeating("TomeDrain", 0, (1f / currentWeapon.chargeLostRate)); // Invokes the func TomeDrain(), instantly once, then once every (1 sec/TomeChargeRate)
    //            }
    //            else
    //            {
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
    //}
    #endregion

    #region Charge Functions
    //private void BowCharge()
    //{
    //    currentBowCharge++;
    //    if (currentBowCharge > currentWeapon.Max_Charge) currentBowCharge = currentWeapon.Max_Charge;
    //    Debug.Log("Current bow charge: " + currentBowCharge);
    //}

    //private void TomeDrain()
    //{
    //    currentTomeCharge--;
    //    tomeChargedFor++;
    //    Debug.Log("Current tome charge: " + currentTomeCharge);
    //    Debug.Log("Current tome charged for: " + tomeChargedFor);
    //    if (tomeChargedFor % 5 == 0)
    //    {
    //        tomeChargedFor = 0;
    //        if (player.Grounded && player.IsOwner)
    //            StartCoroutine(TomeAttack());
    //        TomeAttack();
    //    }
    //    if (currentTomeCharge < 0)
    //    {
    //        currentTomeCharge = 0;
    //        CancelInvoke("TomeDrain");
    //        Debug.Log("Out of Energy");
    //    }
    //}

    //private void TomeCharge()
    //{
    //    currentTomeCharge++;
    //    if (currentTomeCharge >= currentWeapon.Max_Held_Charge)
    //    {
    //        currentTomeCharge = currentWeapon.Max_Held_Charge;
    //        CancelInvoke("TomeCharge");
    //    }
    //    Debug.Log("Current tome charge: " + currentTomeCharge);
    //}
    #endregion

    #region Weapon Attack Functions

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

    #endregion

}