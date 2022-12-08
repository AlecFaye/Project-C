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

    // Used to check if player should be charging their attack
    private bool IsChannalingAttack = false;

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
        ChannalAttack();
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
                AttackingTime = Hotbar[selectedWeapon].AttackingTime;
                AttackingCooldown = Hotbar[selectedWeapon].AttackingCooldown;
            }
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }

    public void OnAttack()
    {
        if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner) {
            switch (Hotbar[selectedWeapon].weaponType) {
                case WeaponType.None:
                    Debug.Log("Wait stop should be NONE");
                    break;
                case WeaponType.Axe:
                    Debug.Log("Execute the Attack: " + Hotbar[selectedWeapon].weaponType);
                    StartCoroutine(AxeAttack());
                    Debug.Log("Started the Attack: " + Hotbar[selectedWeapon].weaponType); 
                    break;
                case WeaponType.Bow:
                    if (!IsChannalingAttack) IsChannalingAttack = true;
                    else {
                        IsChannalingAttack = false;
                        StartCoroutine(BowAttack());
                    }
                    break;
                case WeaponType.Pickaxe:
                    StartCoroutine(PickaxeAttack());
                    break;
                case WeaponType.Tome:
                    if (!IsChannalingAttack) IsChannalingAttack = true;
                    else IsChannalingAttack = false;
                    break;
            }
        }
    }

    private void ChannalAttack() {
        // Checks if weapon is Bow -> will charge Bow Damage
        if (IsChannalingAttack && Hotbar[selectedWeapon].weaponType == WeaponType.Bow)
        {
            Debug.Log("Charge Bow");
        }
        // Checks if weapon is Tome -> will Damage while weapon is Tome
        if (IsChannalingAttack && Hotbar[selectedWeapon].weaponType == WeaponType.Tome)
        {
            Debug.Log("Charge Tome");
            StartCoroutine(TomeAttack());
        }
    }

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

    private IEnumerator BowAttack() {
        Debug.Log("Execute the Bow Attack");
        CanAttack = false;
        IsAttacking = true;
        player._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation
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


    private void OnHotbar1()
    {
        if (player.IsOwner && CanAttack) {
            selectedWeapon = 0;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }

    private void OnHotbar2()
    {
        if (player.IsOwner && CanAttack) {
            selectedWeapon = 1;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }
        
    private void OnHotbar3()
    {
        if (player.IsOwner && CanAttack) {
            selectedWeapon = 2;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }
    
    private void OnHotbar4()
    {
        if (player.IsOwner && CanAttack){
            selectedWeapon = 3;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }

}