using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;


//namespace WeaponController{
    public class WeaponController : MonoBehaviour
    {
        public ThirdPersonController player; // Refrences the player it's attatched too
                
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
                CanAttack = true;
            }
        }

    }
//}