using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using WeaponController;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController wp;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy" && wp.GetComponent<WeaponController>().IsAttacking) other.GetComponent<CollisionDetected>().Hit();
    }

}
