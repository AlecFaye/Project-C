using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using QFSW.QC;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using static Weapon;


//namespace WeaponController{
public class WeaponController : MonoBehaviour
{
    public ThirdPersonController player; // Refrences the player it's attatched too

    [SerializeField] private Weapon[] Hotbar;

    private int selectedWeapon = 0;

    private Weapon currentWeapon;

    // Makes a list to keep track of which enemies were hit (enemies added by the CollisionDetection Script on weapons)
    public List<Collider> enemiesHitList = new List<Collider>();

    // Stats -> Get overwritten
    private bool CanAttack;
    public bool IsAttacking; // Is public to be called by other functions
    private float AttackingTime;
    private float AttackingCooldown;

    // Channel Variables
    private bool IsChannelingAttack = false; // Used to check if player should be charging their attack
    private float currentBowCharge = 0;
    private float currentTomeCharge = 100;
    private float tomeChargedFor = 0;


    private void Start()
    {
        foreach (Weapon weapon in Hotbar)
        {
            if  (weapon != null) {
                CreateWeapon(weapon);
            }
        }

        SelectWeapon();
    }

    private void CreateWeapon(Weapon weapon)
    {
        Transform tempWeapon = Instantiate(weapon.weaponModel, Vector3.zero, Quaternion.identity); // Creates the weapon in the hotbar slot
        tempWeapon.transform.SetParent(this.transform); // Sets this gameobject to the parent of the 
        tempWeapon.transform.localPosition = new Vector3(0.034f, -0.046f, 0.2f); // Sets position to hand
        tempWeapon.transform.localRotation = Quaternion.Euler(15f, -90f, 100f); // Sets rotation to hand
        weapon._player = player;
    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                CanAttack = Hotbar[selectedWeapon].CanAttack;
                IsAttacking = Hotbar[selectedWeapon].IsAttacking;
                AttackingTime = Hotbar[selectedWeapon].attackingTime;
                AttackingCooldown = Hotbar[selectedWeapon].attackingCooldown;

                currentWeapon = Hotbar[selectedWeapon];
            }
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }

    public void OnAttack()
    {
        switch (Hotbar[selectedWeapon].weaponType) {
            case WeaponType.None:
                Debug.Log("Wait stop should be NONE");
                break;

            case WeaponType.Axe:
                if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
                    StartCoroutine(AxeAttack());
                break;

            case WeaponType.Bow:
                if (!IsChannelingAttack) {
                    IsChannelingAttack = true;
                    currentBowCharge = 0;
                    InvokeRepeating("BowCharge", 0f, (1f / currentWeapon.chargeGainedRate)); // Invokes the func BowCharge(), instantly once, then once every (sec/BowChargeRate)
                }
                else {
                    IsChannelingAttack = false;
                    CancelInvoke("BowCharge");
                    if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
                        StartCoroutine(BowAttack(currentBowCharge));
                }
                break;

            case WeaponType.Pickaxe:
                if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
                    StartCoroutine(PickaxeAttack());
                break;

            case WeaponType.Tome:
                if (!IsChannelingAttack) {
                    IsChannelingAttack = true;
                    tomeChargedFor = 0;
                    CancelInvoke("TomeCharge");
                    InvokeRepeating("TomeDrain", (1f / currentWeapon.chargeLostRate), (1f / currentWeapon.chargeLostRate)); // Invokes the func TomeDrain(), once when the normal attack time should trigger, then once every (1 sec/TomeChargeRate)
                }
                else {
                    IsChannelingAttack = false;
                    CancelInvoke("TomeDrain");
                    InvokeRepeating("TomeCharge", 0f, (1f / currentWeapon.chargeLostRate)); // Invokes the func TomeCharge(), instantly once, then once every (1 sec/TomeChargeRate)
                }
                CanAttack = !IsChannelingAttack;
                IsAttacking = IsChannelingAttack;
                break;
        }   
    }

    private void BowCharge() {
        currentBowCharge++;
        if (currentBowCharge > currentWeapon.Max_Charge) currentBowCharge = currentWeapon.Max_Charge;
        //Debug.Log("Current bow charge: " + currentBowCharge);
    }

    private void TomeDrain() {
        currentTomeCharge--;
        tomeChargedFor++;
        //Debug.Log("Current tome charge: " + currentTomeCharge);
        //Debug.Log("Current tome charged for: " + tomeChargedFor);
        if (tomeChargedFor % 5 == 0){
            tomeChargedFor = 0;
            StartCoroutine(TomeAttack());
        }
        if (currentTomeCharge < 0) {
            currentTomeCharge = 0;
            //Debug.Log("Out of Energy");
        }
    }

    private void TomeCharge() {
        currentTomeCharge++;
        if (currentTomeCharge >= currentWeapon.Max_Held_Charge) { 
            currentTomeCharge = currentWeapon.Max_Held_Charge;
            CancelInvoke("TomeCharge");
        }
        //Debug.Log("Current tome charge: " + currentTomeCharge);
    }

    #region Weapon Attack Functions
    private IEnumerator AxeAttack() {
        Debug.Log("Execute the Axe Attack");
        CanAttack = false;
        IsAttacking = true;
        player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
        yield return new WaitForSeconds(AttackingTime);
        IsAttacking = false;
        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        CanAttack = true;
    }
    private IEnumerator BowAttack(float chargeValue) {
        Debug.Log("Execute the Bow Attack");
        CanAttack = false;
        IsAttacking = true;

        Vector3 aimDir = (player.mouseWorldPosition - player._projectileSpawn.position).normalized;
        Transform tempArrow = Instantiate(currentWeapon._arrowType.arrowModel, player._projectileSpawn.position, Quaternion.LookRotation(aimDir, Vector3.up));
        tempArrow.GetComponent<ArrowFunction>().Create(
            currentWeapon._arrowType.travelSpeed * (currentBowCharge/100), // Speed of arrow (travelSpeed) * by charge value (0% - 100%)
            (currentWeapon._arrowType.damageValue + (currentWeapon.damageValue * (currentBowCharge/100))) // Damage of arrow (Arrow Damage + [bow damage * charge value {0% - 100%}] = total damage
            );
        
        //player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
        
        yield return new WaitForSeconds(AttackingTime);
        IsAttacking = false;
        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        CanAttack = true;
    }
    private IEnumerator PickaxeAttack() {
        Debug.Log("Execute the Pickaxe Attack");
        CanAttack = false;
        IsAttacking = true;
        player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
        yield return new WaitForSeconds(AttackingTime);
        IsAttacking = false;
        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        CanAttack = true;
    }
    private IEnumerator TomeAttack() {
        Debug.Log("Execute the Tome Attack");
        
        //player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
        yield return new WaitForSeconds(AttackingTime);
        IsAttacking = false;
        yield return new WaitForSeconds(AttackingCooldown);
        enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
        CanAttack = true;
    }
    #endregion

    #region Hotbar Inputs
    private void OnHotbar1()
    {
        if (player.IsOwner && CanAttack && !IsChannelingAttack) {
            selectedWeapon = 0;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }

    private void OnHotbar2()
    {
        if (player.IsOwner && CanAttack && !IsChannelingAttack) {
            selectedWeapon = 1;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }
        
    private void OnHotbar3()
    {
        if (player.IsOwner && CanAttack && !IsChannelingAttack) {
            selectedWeapon = 2;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }
    
    private void OnHotbar4()
    {
        if (player.IsOwner && CanAttack && !IsChannelingAttack){
            selectedWeapon = 3;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }
    #endregion
}