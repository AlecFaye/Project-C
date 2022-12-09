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

    // Makes a list to keep track of which enemies were hit (enemies added by the CollisionDetection Script on weapons)
    public List<Collider> enemiesHitList = new List<Collider>();

    // Stats -> Get overwritten
    private bool CanAttack;
    public bool IsAttacking; // Is public to be called by other functions
    private float AttackingTime;
    private float AttackingCooldown;

    // Channel Variables
    private bool IsChannelingAttack = false; // Used to check if player should be charging their attack
    private float currentCharge;


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

    private void Update()
    {
        ChannelAttack();
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
                    currentCharge = 0;
                }
                else
                {
                    IsChannelingAttack = false;
                    if (!CanAttack && IsAttacking && !player.Grounded && !player.IsOwner) return;
                    StartCoroutine(BowAttack(currentCharge));
                }
                break;
            case WeaponType.Pickaxe:
                if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner)
                    StartCoroutine(PickaxeAttack());
                break;
            case WeaponType.Tome:
                if (!IsChannelingAttack)
                    IsChannelingAttack = true;
                else 
                    IsChannelingAttack = false;
                break;
        }   
    }

    private void ChannelAttack() {
        Weapon currentWeapon = Hotbar[selectedWeapon];
        // Checks if weapon is Bow -> will charge Bow Damage
        if (IsChannelingAttack && currentWeapon.weaponType == WeaponType.Bow)
        {
            if (currentCharge < currentWeapon.Max_Charge)
                currentCharge += currentWeapon.chargeGainedRate * 10f * Time.deltaTime;
            else
            {
                currentCharge = currentWeapon.Max_Charge;
            }
            Debug.Log(currentCharge);
        }
        // Checks if weapon is Tome -> will Damage while weapon is Tome
        if (IsChannelingAttack && currentWeapon.weaponType == WeaponType.Tome)
        {
            Debug.Log("Charge Tome");
            TomeAttack();
        }
    }

    #region Execute Attack Functions
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
        Transform tempArrow = Instantiate(Hotbar[selectedWeapon]._arrowType.arrowModel, player._projectileSpawn.position, Quaternion.LookRotation(aimDir, Vector3.up));
        tempArrow.GetComponent<ArrowFunction>().Create(10f, Hotbar[selectedWeapon].damageValue * (currentCharge/100)); // Input Proper Speed here
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
        CanAttack = false;
        IsAttacking = true;
        player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
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