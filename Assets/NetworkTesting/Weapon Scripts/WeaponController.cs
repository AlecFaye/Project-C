using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using QFSW.QC;
using Unity.VisualScripting;
using UnityEngine.InputSystem;


//namespace WeaponController{
public class WeaponController : MonoBehaviour
{
    public ThirdPersonController player; // Refrences the player it's attatched too

    [SerializeField] private Weapon[] Hotbar;

    private int selectedWeapon = 0;

    // Stats -> Get overwritten
    private bool CanAttack;
    public bool IsAttacking; // Is public to be called by other functions
    private float AttackingTime;
    private float AttackingCooldown;

    public List<Collider> enemiesHitList = new List<Collider>(); // Makes a list to keep track of which enemies were hit (enemies added by the CollisionDetection Script on weapons)

    private void Start()
    {
        foreach (Weapon weapon in Hotbar)
        {
            if  (weapon != null) {
                Transform tempWeapon = Instantiate(weapon.weaponModel, Vector3.zero, Quaternion.identity); // Creates the weapon in the hotbar slot
                tempWeapon.transform.SetParent(this.transform); // Sets this gameobject to the parent of the 
                tempWeapon.transform.localPosition = new Vector3(0.04f, -0.1f, 0.2f); // Sets position to hand
                tempWeapon.transform.localRotation = Quaternion.Euler(0f, 180f, 90f); // Sets rotation to hand
            }
        }

        SelectWeapon();
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

    private IEnumerator OnAttack() 
        {
            if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner){
                CanAttack = false;
                IsAttacking = true;
                Hotbar[selectedWeapon].Attack(); // Will call the currently selected weapon's attack animation
                //player._animator.SetTrigger("Attack"); // Being cut out to test new attack system
                //player.transform.rotation = Quaternion.Euler(0.0f, player._cinemachineTargetYaw, 0.0f); // Rotates the player to where the camera is facing (only on y axis)
                yield return new WaitForSeconds(AttackingTime);
                IsAttacking = false;
                yield return new WaitForSeconds(AttackingCooldown);
                enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
                CanAttack = true;
            }
        }

    private void OnHotbar1()
    {
        if (player.IsOwner){
            selectedWeapon = 0;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }

    private void OnHotbar2()
    {
        if (player.IsOwner){
            selectedWeapon = 1;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }
        
    private void OnHotbar3()
    {
        if (player.IsOwner){
            selectedWeapon = 2;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }
    
    private void OnHotbar4()
    {
        if (player.IsOwner){
            selectedWeapon = 3;
            SelectWeapon();
            Debug.Log(Hotbar[selectedWeapon].weaponName);
        }
    }

}