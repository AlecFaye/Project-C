using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class CollisionDetection : MonoBehaviour
{
    public ThirdPersonController player;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy" && player.IsAttacking) other.GetComponent<CollisionDetected>().Hit();
    }

}
