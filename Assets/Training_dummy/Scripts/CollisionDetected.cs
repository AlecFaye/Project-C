using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetected : MonoBehaviour
{
    public Animator _animator;

    public void Hit(){
        _animator.SetTrigger("Hit");
    }
}
