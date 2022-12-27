using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ArrowFunction : MonoBehaviour
{
    private Transform anchor; // Used to attach arrow and hit object (Scale breaks the arrow if connected directly)

    private Rigidbody arrowRigidbody;

    private IDamageable playerStats;

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

    public void Create(IDamageable playerStats, float speed, float damage)
    {
        this.playerStats = playerStats;
        damageValue = damage;
        arrowRigidbody.velocity = transform.forward * speed;
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        Debug.Log("Arrow Hit");
        if (!IsTarget) {
            CreateAnchor(other);

            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(playerStats, damageValue, Weapon.WeaponType.Bow);
            }

            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }
        
    }

    // Will be used to attach arrow to target
    private void CreateAnchor(Collider other) {
        IsTarget = false;
        arrowRigidbody.velocity = Vector3.zero;
        transform.SetParent(other.transform);
    }
}
