using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class HotbarController : MonoBehaviour
{
    #region Variables

    public ThirdPersonController player; // Refrences the player it's attatched too
    public WeaponController currentWeapon; // Refrences the current weapon's WeaponController Script

    [SerializeField] private Transform[] HotbarSlots;
    private int selectedWeapon = 0;

    private bool CanAttack = true; // Used to check if player can attack
    private bool IsAttacking = false; // Used to check if player is attacking
    
    private bool IsHoldingAttack = false; // Used to check if player holding the button

    #endregion

    [SerializeField] private Transform[] Hotbar; // this is for testing

    #region Start Function(s)

    private void Start() {
        int hotbarSlot = 0;
        foreach (Transform weapon in Hotbar) {
            if (weapon != null) CreateWeapon(weapon, hotbarSlot);
            hotbarSlot++;
        } 
        
        SelectWeapon();
    }

    #endregion

    #region Weapon Creation Functions

    private void CreateWeapon(Transform weapon, int hotbarSlot) {
        Transform tempWeapon = Instantiate(weapon, this.transform.position, Quaternion.identity); // Creates the weapon in the hotbar slot
        tempWeapon.transform.SetParent(HotbarSlots[hotbarSlot]); // Sets this gameobject to the parent of the 
        tempWeapon.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Sets rotation to hand
        
        SetOwnerofWeapon(tempWeapon); // Set Player of weapon to weapon
    }

    private void SetOwnerofWeapon(Transform weapon) {
        if (weapon.TryGetComponent<WeaponController>(out WeaponController weaponController))
            weaponController.owner = player;
    }

    #endregion

    #region Weapon Creation Functions

    private void SelectWeapon() {
        int position = 0;
        foreach (Transform slot in HotbarSlots) {
            if (position == selectedWeapon) {
                slot.gameObject.SetActive(true);
                //currentWeapon = slot.GetChild(0).GetComponent<WeaponController>();
            }
            else slot.gameObject.SetActive(false);
            
            position++;
        }
    }

    #endregion
   
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
}