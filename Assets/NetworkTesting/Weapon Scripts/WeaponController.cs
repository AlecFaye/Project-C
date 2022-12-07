using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using QFSW.QC;
using Unity.VisualScripting;


//namespace WeaponController{
public class WeaponController : MonoBehaviour
    {
        public ThirdPersonController player; // Refrences the player it's attatched too

        [SerializeField] private Transform[] Hotbar;

        private int selectedWeapon = 0;

        // Player Attack Stats
        [Header("Attack")]
        [Tooltip("If the character can Attack or not.")]
        public bool CanAttack = true;

        [Tooltip("If the character is Attacking or not.")]
        public bool IsAttacking = false;
        
        [Tooltip("How long the Attack goes for (float).")]
        public float AttackingTime = 0.8f;
                
        [Tooltip("Attack Cooldown value (float).")]
        public float AttackingCooldown = 1f;

        public List<Collider> enemiesHitList = new List<Collider>(); // Makes a list to keep track of which enemies were hit (enemies added by the CollisionDetection Script on weapons)


    private void Start()
    {
        foreach (Transform weapon in Hotbar)
        {
            if  (weapon != null) {
                Transform tempWeapon = Instantiate(weapon, Vector3.zero, Quaternion.identity);
                tempWeapon.transform.SetParent(this.transform);
                tempWeapon.transform.localPosition = new Vector3(0.04f, -0.1f, 0.2f);
                Debug.Log("Weapon Spawn Rotation: " + tempWeapon.transform.rotation);
                tempWeapon.transform.localRotation = Quaternion.Euler(0f, 180f, 90f);
                Debug.Log("Weapon Post Rotation: " + tempWeapon.transform.rotation);
                Debug.Log("Weapon Post Rotation X: " + tempWeapon.transform.rotation.x);
                Debug.Log("Weapon Post Rotation Y: " + tempWeapon.transform.rotation.y);
                Debug.Log("Weapon Post Rotation Z: " + tempWeapon.transform.rotation.z);
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
                weapon.gameObject.SetActive(true);
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
                player._animator.SetTrigger("Attack");
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
                Debug.Log("Yeet 1");
                selectedWeapon = 0;
                SelectWeapon();
            }
        }

        private void OnHotbar2()
        {
            if (player.IsOwner){
                Debug.Log("Yeet 2");
            selectedWeapon = 1;
                SelectWeapon();
            }
        }

    }