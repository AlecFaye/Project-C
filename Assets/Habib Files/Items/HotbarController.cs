using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.GridLayoutGroup;


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

    //[SerializeField] private Transform ref_RightHand; // Decide if this should be a transform or the target
    //[SerializeField] private Transform ref_LeftHand; // Might keep only left hand changes and apply those
    // private const int axeAttackLayer = 1;
    // private const int bowAttackLayer = 2;
    // private const int pickaxeAttackLayer = 3;
    // private const int tomeAttackLayer = 4;

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
        weapon.gameObject.SetActive(false);
        Transform tempWeapon = Instantiate(weapon, this.transform.position, Quaternion.identity); // Creates the weapon in the hotbar slot
        tempWeapon.transform.SetParent(HotbarSlots[hotbarSlot]); // Sets this gameobject to the parent of the 
        tempWeapon.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Sets rotation to hand

        tempWeapon.gameObject.SetActive(true);
    }

    #endregion

    #region Hotbar Functions

    private void OnHotbar1() { SwitchHotBar(0); }
    private void OnHotbar2() { SwitchHotBar(1); }
    private void OnHotbar3() { SwitchHotBar(2); }
    private void OnHotbar4() { SwitchHotBar(3); }
    private void OnHotbar5() { SwitchHotBar(4); }
    private void OnHotbar6() { SwitchHotBar(5); }

    private void SwitchHotBar(int hotbarNum) {
        if (currentWeapon.SwitchableCheck()) {
            selectedWeapon = hotbarNum;
            SelectWeapon();
        }
    }
    private void SelectWeapon() {
        Transform selectedSlot = null;
        int position = 0;
        foreach (Transform slot in HotbarSlots) {
            DisableWeapon(slot);
            if (position == selectedWeapon)
                selectedSlot = slot;
            position++;
        }
        EnableWeapon(selectedSlot);
    }

    private void EnableWeapon(Transform slot) {
        slot.gameObject.SetActive(true);
        currentWeapon = slot.GetChild(0).GetComponent<WeaponController>();
        player.currentWeapon = slot.GetChild(0).GetComponent<WeaponController>();

        SetAnimationLayers((int)currentWeapon.weapon.weaponType);
    }
    private void DisableWeapon(Transform slot) { slot.gameObject.SetActive(false); }

    private void SetAnimationLayers(int selectedLayer) {
        for (int layer = 1; layer < player._animator.layerCount; layer++) { player._animator.SetLayerWeight(layer, 0); }

        player._animator.SetLayerWeight(selectedLayer, 1);
    }

    #endregion
}