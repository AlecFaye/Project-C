using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ArrowFunction : MonoBehaviour
{
    private Transform anchor; // Used to attach arrow and hit object (Scale breaks the arrow if connected directly)

    private Rigidbody arrowRigidbody;

    public float damageValue;

    private bool IsTarget = false; // Checks if the arrow has hit a target (true == max one target hit, false == max one target not hit yet)

    private void Awake()
    {
        arrowRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Will set rotation and position based on anchor when added
    }

    public void Create(float speed, float damage)
    {
        damageValue = damage;
        arrowRigidbody.velocity = transform.forward * speed;
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        Debug.Log("Arrow Hit");
        if (!IsTarget)
        {
            if (other.tag != "Player" && other.tag != "Item")
            {
                IsTarget = false;
                //Debug.Log("Arrow damage: " + damageValue);
                arrowRigidbody.velocity = Vector3.zero;
                transform.SetParent(other.transform);

                CreateAnchor(other);

                // Triggers if the enemy has the enemy tag
                if (other.CompareTag("Enemy")) {
                    CollisionDetected collisionDetected = other.GetComponent<CollisionDetected>();

                    if (collisionDetected)
                        collisionDetected.Hit(damageValue, Weapon.WeaponType.Bow); // Runs the funtion "Hit" in the other objects CollisionDetected script
                }

                yield return new WaitForSeconds(5);
                Destroy(gameObject);
            }
        }
    }

    // Will be used to attach arrow to target
    private void CreateAnchor(Collider other) {
        //other.gameObject
    }
}
