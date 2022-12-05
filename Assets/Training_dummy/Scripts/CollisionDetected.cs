using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    public Animator _animator;

    // Triggers when Collision Detection script detectst the enemy 
    public void Hit(){
        _animator.SetTrigger("Hit");
    }
}
