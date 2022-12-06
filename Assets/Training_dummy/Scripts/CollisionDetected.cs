using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    public Animator _animator;
    public Transform _damagePopupSpawn;

    // Triggers when Collision Detection script detectst the enemy 
    public void Hit(){
        _animator.SetTrigger("Hit");
        DamagePopup.Create(_damagePopupSpawn.position, 10); // Creates the popup using the paramaters for (Transform position, int DamageValue)
    }
}
