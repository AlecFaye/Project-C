using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class AxeController : MonoBehaviour
{
    public ThirdPersonController owner;


    public void OnAxeAttack() { 
            if (CanAttack && !IsAttacking && owner.Grounded && owner.IsOwner) {
                owner.IsAttacking = true;
                owner.IsConstantAim = false;
                owner.aimTarget = owner.mouseWorldPosition;
                owner.RotatePlayerToCamera();

                StartCoroutine(AxeAttack());
        }

    private IEnumerator AxeAttack() {
            CanAttack = false;
            IsAttacking = true;

            owner._animator.SetTrigger("Axe Attack"); // Will call the currently selected weapon's attack animation

            yield return new WaitForSeconds(AttackingTime);
            IsAttacking = false;
            owner.IsAttacking = false;
            yield return new WaitForSeconds(AttackingCooldown);
            enemiesHitList = new List<Collider>(); // Resets the list of enemies so that they can be hit again
            CanAttack = true;
        }
    }
