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

        public List<Collider> enemiesHitList = new List<Collider>(); // Makes a list to keep track of which enemies were hit (enemies added by the CollisionDetection Script on weapons)


        private IEnumerator OnAttack() 
        {
            if (CanAttack && !IsAttacking && player.Grounded && player.IsOwner){
                CanAttack = false;
                IsAttacking = true;
                player._animator.SetTrigger("Attack");
                //player.transform.rotation = Quaternion.Euler(0.0f, player._cinemachineTargetYaw, 0.0f); // Rotates the player to where the camera is facing (only on y axis)
                yield return new WaitForSeconds(AttackingTime);
                IsAttacking = false;
                //Debug.Log("Enemy list Before Reset: " + enemiesHitList);
                yield return new WaitForSeconds(AttackingCooldown);
                enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
                //Debug.Log("Enemy list After Reset: " + enemiesHitList);
                CanAttack = true;
            }
        }

    }
//}